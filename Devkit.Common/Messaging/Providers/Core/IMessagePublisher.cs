using MassTransit;

namespace Devkit.Common.Messaging.Providers.Core;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default);
}

public class MessagePublisher(IPublishEndpoint publishEndpoint) : IMessagePublisher
{
    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));

        await publishEndpoint.Publish(message, cancellationToken);
    }
}