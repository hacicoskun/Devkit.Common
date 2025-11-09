using Devkit.Common.Messaging.Options;
using Devkit.Common.Messaging.Providers.Core;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Devkit.Common.Messaging.Providers.RabbitMQ;

public class RabbitMqProvider : IMessageBusProvider
{
    public string ProviderName => "RabbitMQ";

    public void Configure(IServiceCollection services, IConfiguration config, Action<IBusRegistrationConfigurator>? configure = null)
    {
        var options = config.GetSection("Messaging").Get<MessageBusOptions>()
                      ?? throw new InvalidOperationException("Messaging configuration missing.");


        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();


            configure?.Invoke(x);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(options.Host, options.VirtualHost ?? "/", h =>
                {
                    if (!string.IsNullOrWhiteSpace(options.Username))
                        h.Username(options.Username);
                    if (!string.IsNullOrWhiteSpace(options.Password))
                        h.Password(options.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

    }
}