using System.Reflection;
using Devkit.Common.Messaging.Core;
using Devkit.Common.Messaging.Options;
using Devkit.Common.Messaging.Outbox;
using Devkit.Common.Messaging.Providers.Kafka;
using Devkit.Common.Messaging.Providers.RabbitMQ;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Devkit.Common.Messaging.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddMessaging(this IServiceCollection services, IConfiguration configuration, Assembly? consumerAssembly = null, bool useConsumers = true, Action<IBusRegistrationConfigurator>? configureBus = null)
    {
        AddMessagingCore(services, configuration, consumerAssembly, useConsumers, outboxConfigurator: null, configureBus);
    }

    public static void AddMessagingWithOutbox<TContext>(this IServiceCollection services, IConfiguration configuration, Assembly? consumerAssembly = null, bool useConsumers = true, Action<IBusRegistrationConfigurator>? configureBus = null)
        where TContext : DbContext
    {
        AddMessagingCore(services, configuration, consumerAssembly, useConsumers, outboxConfigurator: x => x.AddEfCoreOutbox<TContext>(), configureBus);
    }

    private static void AddMessagingCore(IServiceCollection services, IConfiguration configuration, Assembly? consumerAssembly, bool useConsumers, Action<IBusRegistrationConfigurator>? outboxConfigurator,
        Action<IBusRegistrationConfigurator>? configureBus)
    {
        var options = configuration.GetSection("Messaging").Get<MessageBusOptions>()
                      ?? throw new InvalidOperationException("Messaging configuration missing.");

        consumerAssembly ??= Assembly.GetCallingAssembly();

        void Configure(IBusRegistrationConfigurator x)
        {
            if (useConsumers)
                x.AddConsumers(consumerAssembly);

            outboxConfigurator?.Invoke(x);

            configureBus?.Invoke(x);
        }

        switch (options.Provider!.ToLowerInvariant())
        {
            case "rabbitmq":
                new RabbitMqProvider().Configure(services, configuration, Configure);
                services.AddScoped<RabbitMqPublisher>();
                break;

            case "kafka":
                new KafkaProvider().Configure(services, configuration, Configure);
                services.AddScoped<KafkaPublisher>();
                break;

            default:
                throw new NotSupportedException($"Unsupported provider: {options.Provider}");
        }

        services.AddScoped<IPublisher, Publisher>();
    }
}
