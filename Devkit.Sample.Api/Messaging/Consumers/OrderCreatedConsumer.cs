using Devkit.Sample.Api.Messaging.Models.Events;
using MassTransit;

namespace Devkit.Sample.Api.Messaging;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        Console.WriteLine($"📨 Consumer: Order received — {context.Message.Product} ({context.Message.Price}₺)");

        await context.Publish(new OrderProcessedEvent(context.Message.OrderId, DateTime.UtcNow));
    }
}