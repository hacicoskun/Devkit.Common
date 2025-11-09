namespace Devkit.Sample.Api.Data.Entities;

public sealed class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Product { get; set; } = default!;
    public decimal Price { get; set; }
}