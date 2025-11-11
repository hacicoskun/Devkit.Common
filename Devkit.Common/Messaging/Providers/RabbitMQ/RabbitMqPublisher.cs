using Devkit.Common.Messaging.Providers.Core;
using MassTransit;

namespace Devkit.Common.Messaging.Providers.RabbitMQ;

public class RabbitMqPublisher(IBus bus): IPublisher
{
    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : class
        => bus.Publish(message, cancellationToken);

    public Task SendAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : class
        => bus.Send(message, cancellationToken);

    public async Task<TResponse> RequestAsync<TRequest, TResponse>(
        TRequest message, CancellationToken cancellationToken = default)
        where TRequest : class where TResponse : class
    {
        var client = bus.CreateRequestClient<TRequest>();
        var response = await client.GetResponse<TResponse>(message, cancellationToken);
        return response.Message;
    }
}