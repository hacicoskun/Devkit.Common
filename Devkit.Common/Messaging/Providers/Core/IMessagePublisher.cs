using MassTransit;

namespace Devkit.Common.Messaging.Providers.Core;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default);
}

public class MessagePublisher(IBus bus) : IMessagePublisher
{
    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));

        await bus.Publish(message, cancellationToken);
    }
}