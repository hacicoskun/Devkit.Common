using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Devkit.Common.Messaging.Core;

public interface IMessageBusProvider
{
    string ProviderName { get; }

    void Configure(IServiceCollection services, IConfiguration config, Action<IBusRegistrationConfigurator>? configure = null);
}