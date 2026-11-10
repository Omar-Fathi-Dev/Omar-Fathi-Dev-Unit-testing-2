using Bogus;
using Simulation.Checkout.Domain;

namespace Simulation.Checkout.Tests.TestData;

public sealed class OrderFaker
{
    private readonly Faker<Order> _orderFaker;

    public OrderFaker()
    {
        Faker<OrderItem> itemFaker = new Faker<OrderItem>()
            .RuleFor(item => item.ProductId, f => f.Random.Guid())
            .RuleFor(item => item.Quantity, f => f.Random.Int(1, 10))
            .RuleFor(item => item.UnitPrice, f => f.Random.Decimal(0, 1000))
            .RuleFor(item => item.ProductName, f => f.Commerce.ProductName())
            .UseSeed(1234);

        _orderFaker = new Faker<Order>()
            .RuleFor(order => order.CustomerName, f => f.Name.FullName())
            .RuleFor(order => order.CustomerEmail, f => f.Internet.Email())
            .RuleFor(order => order.Items, _ => itemFaker.Generate(2))
            .UseSeed(1234);
    }

    public Order Generate()
    {
        return _orderFaker.Generate();
    }
}