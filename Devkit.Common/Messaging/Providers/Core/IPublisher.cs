namespace Devkit.Common.Messaging.Providers.Core;

public interface IPublisher
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : class;

    Task SendAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : class;

    Task<TResponse> RequestAsync<TRequest, TResponse>(
        TRequest message,
        CancellationToken cancellationToken = default)
        where TRequest : class
        where TResponse : class;
}