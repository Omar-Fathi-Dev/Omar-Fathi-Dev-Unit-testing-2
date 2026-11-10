using AutoFixture;
using FluentAssertions;
using Moq;
using Simulation.Checkout.Application;
using Simulation.Checkout.Application.Abstractions;
using Simulation.Checkout.Domain;
using Simulation.Checkout.Tests.TestData;
namespace Simulation.Checkout.Tests;

public class TestDataTests
{
    
    //Task 3 : 3.1 --->    AutoFixture creates the order. Customize it so quantity > 0 and price ≥ 0
    [Fact]
    public async Task CheckoutAsync_AutoFixtureOrderWithValidItems_Succeeds()
    {
        // Arrange
        Fixture fixture = new Fixture();
        fixture.Customize(new ValidOrderCustomization());
        Order order = fixture.Create<Order>();
        
        Mock<IInventoryService> inventoryMock = new Mock<IInventoryService>();
        Mock<IOrderRepository> orderRepositoryMock = new Mock<IOrderRepository>();
        Mock<INotificationService> notificationMock = new Mock<INotificationService>();
        inventoryMock.Setup(x => x.IsAvailableAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        CheckoutService checkoutService = new CheckoutService(orderRepositoryMock.Object , 
            inventoryMock.Object, notificationMock.Object);
        
        
        // Act
        var receipt = await checkoutService.CheckoutAsync(order);
        
        
        // Assert
        receipt.Should().NotBeNull();
        receipt.Total.Should().BeGreaterThan(0);
    }
    
    
    //Task 3 : 3.2 --->      Bogus creates a realistic customer (name, email) with a fixed seed.
    [Fact]
    public async Task CheckoutAsync_BogusCustomer_PreservesNameAndEmailInReceipt()
    {
        
        // Arrange
        OrderFaker orderFaker = new OrderFaker();
        Order order = orderFaker.Generate();
        
        Mock<IInventoryService> inventoryMock = new Mock<IInventoryService>();
        Mock<IOrderRepository> orderRepositoryMock = new Mock<IOrderRepository>();
        Mock<INotificationService> notificationMock = new Mock<INotificationService>();
        inventoryMock.Setup(x => x.IsAvailableAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        CheckoutService checkoutService = new CheckoutService(orderRepositoryMock.Object , 
            inventoryMock.Object, notificationMock.Object);
        
        
        // Act
        var receipt = await checkoutService.CheckoutAsync(order);
        
        
        // Assert
        receipt.CustomerName.Should().Be(order.CustomerName);
        receipt.CustomerEmail.Should().Be(order.CustomerEmail);


    }
    
}