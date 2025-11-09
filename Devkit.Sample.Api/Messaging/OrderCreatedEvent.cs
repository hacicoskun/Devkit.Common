namespace Devkit.Sample.Api.Messaging;

public record OrderCreatedEvent(Guid OrderId, string Product, decimal Price);
