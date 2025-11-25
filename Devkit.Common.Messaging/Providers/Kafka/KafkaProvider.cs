using Confluent.Kafka;
using Devkit.Common.Messaging.Core;
using Devkit.Common.Messaging.Options;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Devkit.Common.Messaging.Providers.Kafka
{
    public class KafkaProvider : IMessageBusProvider
    {
        public string ProviderName => "Kafka";

        public void Configure(IServiceCollection services, IConfiguration config, Action<IBusRegistrationConfigurator>? configure = null)
        {
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                configure?.Invoke(x);

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });

                x.AddRider(rider =>
                {
                    var consumerAssembly = configure?.Method?.DeclaringType?.Assembly ?? typeof(KafkaProvider).Assembly;
                    rider.AddConsumers(consumerAssembly);

                    rider.UsingKafka((context, k) =>
                    {
                        var options = config.GetSection("Messaging").Get<MessageBusOptions>()
                                      ?? throw new InvalidOperationException("Messaging configuration missing.");

                        k.Host(options.Host, h =>
                        {
                            if (!string.IsNullOrWhiteSpace(options.Username) && !string.IsNullOrWhiteSpace(options.Password))
                            {
                                h.UseSasl(sasl =>
                                {
                                    sasl.Mechanism = SaslMechanism.Plain;
                                    sasl.Username = options.Username;
                                    sasl.Password = options.Password;
                                });
                            }
                        });
 
                    });
                });
            });
        }
    }
}
