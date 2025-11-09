using Devkit.Common.Messaging.Providers.Core;
using Devkit.Sample.Api.Data;
using Devkit.Sample.Api.Data.Entities;
using Devkit.Sample.Api.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Devkit.Sample.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController(AppDbContext context, IMessagePublisher bus) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder(string product, decimal price)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var order = new Order { Product = product, Price = price };
            context.Orders.Add(order);
            
            await bus.PublishAsync(new OrderCreatedEvent(order.Id, order.Product, order.Price)); 

            await transaction.CommitAsync(); 

            return Ok(new { order.Id, order.Product, order.Price });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            
            return BadRequest(new { message = "Sipariş oluşturma başarısız oldu: " + ex.Message });
        }
    }
}