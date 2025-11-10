namespace Devkit.Sample.Api.Messaging;

public record OrderProcessedEvent(Guid OrderId, DateTime ProcessedAt);