using Devkit.Common.Messaging.Providers.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;

namespace Devkit.Common.Messaging.Providers.Kafka;

public class KafkaProvider : IMessageBusProvider
{
    public string ProviderName => "Kafka";

    public void Configure(IServiceCollection services, IConfiguration config, Action<IBusRegistrationConfigurator>? configure = null)
    {
        throw new NotImplementedException("Kafka support is not implemented yet.");
    }
}