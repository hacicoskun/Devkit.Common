using Devkit.Common.Identity.Core.Interfaces;
using Devkit.Common.Messaging.Core;
using Devkit.Sample.Api.Data;
using Devkit.Sample.Api.Data.Entities;
using Devkit.Sample.Api.Messaging.Models.Commands;
using Devkit.Sample.Api.Messaging.Models.Events;
using Devkit.Sample.Api.Messaging.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Devkit.Sample.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MessagingTestController(AppDbContext context, IPublisher bus,IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost("publish")]
    public async Task<IActionResult> PublishOrder(string product, decimal price)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        { 
            var order = new Order { Product = product, Price = price };
            context.Orders.Add(order); 

            await bus.PublishAsync(new OrderCreatedEvent(order.Id, order.Product, order.Price));

            await transaction.CommitAsync();
            return Ok(new { message = "Order published", order.Id, order.Product, order.Price });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return BadRequest(new { message = "Publish failed: " + ex.Message });
        }
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendOrder(Guid orderId)
    {
        try
        {
            var order = await context.Orders.FindAsync(orderId);
            if (order is null)
                return NotFound(new { message = $"Order {orderId} not found" });

            await bus.SendAsync(new GenerateInvoiceCommand(order.Id, order.Price));

            return Ok(new { message = "Command sent successfully", order.Id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Send failed: " + ex.Message });
        }
    }

    [HttpGet("request/{orderId:guid}")]
    public async Task<IActionResult> RequestOrder(Guid orderId)
    {
        try
        {
            var response = await bus.RequestAsync<GetOrderRequest, GetOrderResponse>(
                new GetOrderRequest(orderId)
            );

            return Ok(new
            {
                message = "Response received successfully",
                response.OrderId,
                response.Product,
                response.Price
            });
        }
        catch (Exception ex)
        {
            return NotFound(new { message = "Request failed: " + ex.Message });
        }
    }
}
