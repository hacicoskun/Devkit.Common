using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Devkit.Common.Messaging.Outbox;

public static class OutboxConfigurator
{
    public static void AddEfCoreOutbox<T>(this IBusRegistrationConfigurator cfg)
        where T : DbContext
    {
        cfg.AddEntityFrameworkOutbox<T>(o =>
        {
            o.UseBusOutbox();
            o.UsePostgres();
            o.QueryDelay = TimeSpan.FromSeconds(5);
        });

        cfg.AddConfigureEndpointsCallback((context, name, endpointCfg) =>
        {
            endpointCfg.UseEntityFrameworkOutbox<T>(context);
        });
    }
}