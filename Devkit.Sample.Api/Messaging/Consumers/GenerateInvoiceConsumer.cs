using Devkit.Sample.Api.Messaging.Models.Commands;
using MassTransit;

namespace Devkit.Sample.Api.Messaging;

public class GenerateInvoiceConsumer : IConsumer<GenerateInvoiceCommand>
{
    public async Task Consume(ConsumeContext<GenerateInvoiceCommand> context)
    {
        Console.WriteLine($"[COMMAND] Generating invoice for order {context.Message.OrderId}");
        await Task.CompletedTask;
    }
}