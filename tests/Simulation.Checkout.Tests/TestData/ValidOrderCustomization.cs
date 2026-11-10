using AutoFixture;
using Simulation.Checkout.Domain;

namespace Simulation.Checkout.Tests.TestData;

public sealed class ValidOrderCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<OrderItem>(composer => composer
            .With(item => item.Quantity, 1)
            .With(item => item.UnitPrice, 100m));
    }
}