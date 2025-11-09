using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;

namespace Devkit.Common.Messaging.Providers.Core;

public interface IMessageBusProvider
{
    string ProviderName { get; }
    void Configure(IServiceCollection services, IConfiguration config, Action<IBusRegistrationConfigurator>? configure = null);
}