using System.Reflection;
using Devkit.Common.Messaging.Options;
using Devkit.Common.Messaging.Outbox;
using Devkit.Common.Messaging.Providers.Core;
using Devkit.Common.Messaging.Providers.Kafka;
using Devkit.Common.Messaging.Providers.RabbitMQ;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Devkit.Common.Messaging.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Mesaj altyapısını (RabbitMQ, Kafka vb.) tek noktadan yapılandırır.
    /// Outbox, Consumer ve Publisher desteği opsiyoneldir.
    /// </summary>
    public static void AddMessaging<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        Assembly? consumerAssembly = null,
        bool enableConsumers = true,
        bool enableOutbox = false,
        Action<IBusRegistrationConfigurator>? configureBus = null)
        where TContext : DbContext
    {
        var options = configuration.GetSection("Messaging").Get<MessageBusOptions>()
                      ?? throw new InvalidOperationException("Messaging configuration missing.");

        if (string.IsNullOrWhiteSpace(options.Provider))
            throw new InvalidOperationException("Messaging.Provider is not specified in configuration.");

        consumerAssembly ??= Assembly.GetCallingAssembly();

        switch (options.Provider.ToLowerInvariant())
        {
            case "rabbitmq":
                new RabbitMqProvider().Configure(services, configuration, x =>
                {
                    if (enableConsumers)
                        x.AddConsumers(consumerAssembly);

                    if (enableOutbox)
                        x.AddEfCoreOutbox<TContext>();

                    configureBus?.Invoke(x);
                });
                break;

            case "kafka":
                new KafkaProvider().Configure(services, configuration, x =>
                {
                    if (enableConsumers)
                        x.AddConsumers(consumerAssembly);

                    if (enableOutbox)
                        
                        x.AddEfCoreOutbox<TContext>();

                    configureBus?.Invoke(x);
                });
                break;

            default:
                throw new NotSupportedException($"Unsupported provider: {options.Provider}");
        }

        services.AddScoped<IMessagePublisher, MessagePublisher>();
    }
}
