namespace Devkit.Sample.Api.Messaging.Models.Requests;

public record GetOrderResponse(Guid OrderId, string Product, decimal Price);
