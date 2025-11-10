namespace Devkit.Sample.Api.Messaging.Models.Commands;

public record GenerateInvoiceCommand(Guid OrderId, decimal Amount);
