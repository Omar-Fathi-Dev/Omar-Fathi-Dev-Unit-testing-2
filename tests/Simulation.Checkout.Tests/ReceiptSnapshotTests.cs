using VerifyXunit;
using Xunit;
using Moq;
using FluentAssertions;
using Simulation.Checkout.Domain;
using Simulation.Checkout.Application;
using Simulation.Checkout.Application.Abstractions;

namespace Simulation.Checkout.Tests;

public class ReceiptSnapshotTests
{
    [Fact]
    public async Task CheckoutAsync_ExampleC_MatchesSnapshot()
    {
        // Arrange
        Order order = new()
        {
            CustomerName = "Omar Fathy",
            CustomerEmail = "omar@example.com",
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    ProductName = "PC",
                    UnitPrice = 500m,
                    Quantity = 3
                }
            }
        };

        Mock<IInventoryService> inventoryMock = new();
        Mock<IOrderRepository> orderRepositoryMock = new();
        Mock<INotificationService> notificationMock = new();

        inventoryMock.Setup(x => x.IsAvailableAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        CheckoutService checkoutService = new(orderRepositoryMock.Object, inventoryMock.Object, notificationMock.Object);

        
        // Act
        var receipt = await checkoutService.CheckoutAsync(order);

        
        // Assert
        await Verify(receipt);
    }
}