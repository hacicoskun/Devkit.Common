using MassTransit;

namespace Devkit.Common.Messaging.Providers.Core;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default);
    Task SendAsync<T>(T message, CancellationToken cancellationToken = default);

    Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest message, CancellationToken cancellationToken = default)
        where TRequest : class where TResponse : class;
}

public class MessagePublisher(IBus bus) : IMessagePublisher
{
    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        await bus.Publish(message, cancellationToken);
    }

    public async Task SendAsync<T>(T message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        await bus.Send(message, cancellationToken);
    }

    public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest message,
        CancellationToken cancellationToken = default) where TRequest : class where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(message);

        var client = bus.CreateRequestClient<TRequest>();
        var response = await client.GetResponse<TResponse>(message, cancellationToken);

        return response.Message;
    }
}