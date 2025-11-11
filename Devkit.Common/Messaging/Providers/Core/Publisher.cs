using Devkit.Common.Messaging.Providers.Core;
using Devkit.Common.Messaging.Providers.Kafka;
using Devkit.Common.Messaging.Providers.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Devkit.Common.Messaging.Providers.Core;

public class Publisher  : IPublisher
{
    private readonly IPublisher _provider;

    public Publisher(IServiceProvider sp, IConfiguration config)
    {
        var type = config["Messaging:Provider"]?.ToLowerInvariant();

        _provider = type switch
        {
            "kafka" => sp.GetRequiredService<KafkaPublisher>(),
            "rabbitmq" => sp.GetRequiredService<RabbitMqPublisher>(),
            _ => throw new InvalidOperationException($"Unsupported message provider '{type}'.")
        };
    }

    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : class => _provider.PublishAsync(message, cancellationToken);

    public Task SendAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : class => _provider.SendAsync(message, cancellationToken);

    public Task<TResponse> RequestAsync<TRequest, TResponse>(
        TRequest message, CancellationToken cancellationToken = default)
        where TRequest : class where TResponse : class =>
        _provider.RequestAsync<TRequest, TResponse>(message, cancellationToken);
}