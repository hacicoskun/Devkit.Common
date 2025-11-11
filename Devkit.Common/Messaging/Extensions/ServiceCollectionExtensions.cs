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

namespace Devkit.Common.Messaging.Extensions
{  

    public static class ServiceCollectionExtensions
    {
        private static MessageBusOptions GetMessagingOptions(IConfiguration configuration)
        {
            return configuration.GetSection("Messaging").Get<MessageBusOptions>()
                  ?? throw new InvalidOperationException("Messaging configuration missing.");
        }

        public static void AddRabbitMq<TContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            Assembly? consumerAssembly = null,
            bool enableConsumers = true,
            bool enableOutbox = false,
            Action<IBusRegistrationConfigurator>? configureBus = null)
            where TContext : DbContext
        {
            _ = GetMessagingOptions(configuration);
            
            consumerAssembly ??= Assembly.GetCallingAssembly();

            new RabbitMqProvider().Configure(services, configuration, x =>
            {
                if (enableConsumers)
                    x.AddConsumers(consumerAssembly);

                if (enableOutbox)
                    x.AddEfCoreOutbox<TContext>();

                configureBus?.Invoke(x);
            });

            services.AddScoped<RabbitMqPublisher>();
            services.AddScoped<IPublisher, Publisher>();
            
        }

        public static void AddKafka<TContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            Assembly? consumerAssembly = null,
            bool enableConsumers = true,
            bool enableOutbox = false,
            Action<IBusRegistrationConfigurator>? configureBus = null)
            where TContext : DbContext
        {
            _ = GetMessagingOptions(configuration);
             

            consumerAssembly ??= Assembly.GetCallingAssembly();

            new KafkaProvider().Configure(services, configuration, x =>
            {
                if (enableConsumers)
                    x.AddConsumers(consumerAssembly);

                if (enableOutbox)
                    x.AddEfCoreOutbox<TContext>();

                configureBus?.Invoke(x);
            });

            services.AddScoped<KafkaPublisher>(); 
            services.AddScoped<IPublisher, Publisher>();
        }
    }
}