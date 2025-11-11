using Devkit.Sample.Api.Data;
using Devkit.Sample.Api.Messaging.Models.Requests;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Devkit.Sample.Api.Messaging;

public class GetOrderConsumer(AppDbContext dbContext) : IConsumer<GetOrderRequest>
{
    public async Task Consume(ConsumeContext<GetOrderRequest> context)
    {
        var order = await dbContext.Orders.FirstOrDefaultAsync(x=>x.Id == context.Message.OrderId);

        if (order is null)
            throw new KeyNotFoundException($"Order {context.Message.OrderId} not found");

        await context.RespondAsync(new GetOrderResponse(order.Id, order.Product, order.Price));
    }
}
