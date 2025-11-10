namespace Devkit.Sample.Api.Messaging.Models.Events;

public record OrderCreatedEvent(Guid OrderId, string Product, decimal Price);
