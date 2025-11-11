using Devkit.Common.Messaging.Providers.Core;
using MassTransit;
using MassTransit.KafkaIntegration;
using Microsoft.Extensions.DependencyInjection;

namespace Devkit.Common.Messaging.Providers.Kafka;

public class KafkaPublisher(IServiceProvider serviceProvider) : IPublisher
{
    private readonly ITopicProducerProvider _producerProvider = serviceProvider.GetRequiredService<ITopicProducerProvider>();

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(message);

        var topicName = typeof(T).Name;

        var producer = _producerProvider.GetProducer<T>(new Uri($"topic:{topicName}"));

        await producer.Produce(message, cancellationToken);

        Console.WriteLine($"✅ Kafka message published to topic '{topicName}' ({typeof(T).Name})");
    }

    public Task SendAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : class =>
        throw new NotSupportedException("SendAsync is not supported for KafkaPublisherProvider.");

    public Task<TResponse> RequestAsync<TRequest, TResponse>(
        TRequest message, CancellationToken cancellationToken = default)
        where TRequest : class where TResponse : class =>
        throw new NotSupportedException("Request/Response is not supported for KafkaPublisherProvider.");
}