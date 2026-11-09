using FluentAssertions;
using Moq;
using Simulation.Checkout.Application;
using Simulation.Checkout.Application.Abstractions;
using Simulation.Checkout.Domain;
using Simulation.Checkout.Domain.Exceptions;

namespace Simulation.Checkout.Tests;

public class CheckoutServiceTests
{
    //Task 1 : 1.1 ---> Order with an empty item list
    [Fact]
    public async Task CheckoutAsync_EmptyItems_ThrowsEmptyOrderException()
    {
        // Arrange
        Order order = new()
        {
            CustomerName="Omar Fathy" ,
            CustomerEmail= "Omar@gmail.com",
            Items = new List<OrderItem>()
        };
        Mock<IInventoryService> inventoryMock = new Mock<IInventoryService>();
        Mock<IOrderRepository> orderRepositoryMock = new Mock<IOrderRepository>();
        Mock<INotificationService> notificationMock = new Mock<INotificationService>();
        CheckoutService checkoutService = new CheckoutService(orderRepositoryMock.Object , 
            inventoryMock.Object, notificationMock.Object);
        
        // Act
        Func<Task> act = () => checkoutService.CheckoutAsync(order);
        
        
        // Assert
        await act.Should().ThrowAsync<EmptyOrderException>();
    }
    
    
    //Task 1 : 1.2 ---> quantity 0 and -1 
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task CheckoutAsync_InvalidQuantity_ThrowsInvalidQuantityException(int quantity)
    {
        // Arrange
        Order order = new()
        {
            CustomerName="Omar Fathy" ,
            CustomerEmail= "Omar@gmail.com",
            Items = new List<OrderItem>(){new OrderItem()
            {
                ProductId = Guid.NewGuid(),
                ProductName = "PC",
                UnitPrice = 25000m,
                Quantity = quantity
            }}
        };
        Mock<IInventoryService> inventoryMock = new Mock<IInventoryService>();
        Mock<IOrderRepository> orderRepositoryMock = new Mock<IOrderRepository>();
        Mock<INotificationService> notificationMock = new Mock<INotificationService>();
        CheckoutService checkoutService = new CheckoutService(orderRepositoryMock.Object , 
            inventoryMock.Object, notificationMock.Object);
        
        // Act
        Func<Task> act = () => checkoutService.CheckoutAsync(order);
        
        // Assert
        var result =await act.Should().ThrowAsync<InvalidQuantityException>();
        result.Which.Message.Should().Contain(order.Items.First().ProductName);

    }
    
    
    //Task 1 : 1.3 ---> Price -0.01m
    [Fact]
    public async Task CheckoutAsync_NegativePrice_ThrowsInvalidPriceException()
    {
        // Arrange
        Order order = new()
        {
            CustomerName="Omar Fathy" ,
            CustomerEmail= "Omar@gmail.com",
            Items = new List<OrderItem>(){new OrderItem()
            {
                ProductId = Guid.NewGuid(),
                ProductName = "PC",
                UnitPrice = -0.01m,
                Quantity = 1
            }}
        };
        Mock<IInventoryService> inventoryMock = new Mock<IInventoryService>();
        Mock<IOrderRepository> orderRepositoryMock = new Mock<IOrderRepository>();
        Mock<INotificationService> notificationMock = new Mock<INotificationService>();
        CheckoutService checkoutService = new CheckoutService(orderRepositoryMock.Object , 
            inventoryMock.Object, notificationMock.Object);
        
        // Act
        Func<Task> act = () => checkoutService.CheckoutAsync(order);
        
        
        // Assert
        await act.Should().ThrowAsync<InvalidPriceException>();
    }
    
    
    //Task 1 : 1.4 ---> Price 0 Checkout succeeds
    [Fact]
    public async Task CheckoutAsync_ZeroPrice_Succeeds()
    {
        // Arrange
        Order order = new()
        {
            CustomerName="Omar Fathy" ,
            CustomerEmail= "Omar@gmail.com",
            Items = new List<OrderItem>(){new OrderItem()
            {
                ProductId = Guid.NewGuid(),
                ProductName = "PC",
                UnitPrice = 0m,
                Quantity = 1
            }}
        };
        Mock<IInventoryService> inventoryMock = new Mock<IInventoryService>();
        Mock<IOrderRepository> orderRepositoryMock = new Mock<IOrderRepository>();
        Mock<INotificationService> notificationMock = new Mock<INotificationService>();
        inventoryMock.Setup(x => x.IsAvailableAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        CheckoutService checkoutService = new CheckoutService(orderRepositoryMock.Object , 
            inventoryMock.Object, notificationMock.Object);
        
        
        // Act
        var result = await checkoutService.CheckoutAsync(order);
        
        
        // Assert
        result.Total.Should().Be(0m);
    }
    
    
    //Task 1 : 1.5 ---> discount for subtotal 999.99, 1000, 1500
    [Theory]
    [InlineData(999.99,0)]
    [InlineData(1000,100)]
    [InlineData(1500,100)]
    public async Task CheckoutAsync_Subtotal_DiscountIsCalculatedCorrectly(decimal subtotal , decimal expected)
    {
        // Arrange
        Order order = new()
        {
            CustomerName="Omar Fathy" ,
            CustomerEmail= "Omar@gmail.com",
            Items = new List<OrderItem>(){new OrderItem()
            {
                ProductId = Guid.NewGuid(),
                ProductName = "PC",
                UnitPrice = subtotal,
                Quantity = 1
            }}
        };
        Mock<IInventoryService> inventoryMock = new Mock<IInventoryService>();
        Mock<IOrderRepository> orderRepositoryMock = new Mock<IOrderRepository>();
        Mock<INotificationService> notificationMock = new Mock<INotificationService>();
        inventoryMock.Setup(x => x.IsAvailableAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        CheckoutService checkoutService = new CheckoutService(orderRepositoryMock.Object , 
            inventoryMock.Object, notificationMock.Object);
        
        
        // Act
        var result = await checkoutService.CheckoutAsync(order);
        
        
        // Assert
        result.Subtotal.Should().Be(subtotal);
        result.Discount.Should().Be(expected);
    }
    
    
    //Task 1 : 1.6 --->  Example A and C (above)
    [Theory]
    [InlineData(true ,700 , 0 , 700)]
    [InlineData(false ,1500 , 100 , 1400)]
    public async Task CheckoutAsync_ExampleAAndC_CalculatesCorrectAmounts(bool isExampleA,
        decimal expectedSubtotal , decimal expectedDiscount , decimal  expectedTotal
        )
    {
        // Arrange
        Order order ;
        if (isExampleA)
        {
             order = new()
            {
                CustomerName="Omar Fathy" ,
                CustomerEmail= "Omar@gmail.com",
                Items = new List<OrderItem>(){ new OrderItem()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "PC",
                    UnitPrice = 150,
                    Quantity = 2
                },
                    new OrderItem()
                    {
                        ProductId = Guid.NewGuid(),
                        ProductName = "Phone",
                        UnitPrice = 400,
                        Quantity = 1
                    }
                }
                
                
            };
        }
        else
        {
            order = new()
            {
                CustomerName="Omar Fathy" ,
                CustomerEmail= "Omar@gmail.com",
                Items = new List<OrderItem>(){ new OrderItem()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "PC",
                    UnitPrice = 500,
                    Quantity = 3
                }}
            };
        }
        
        Mock<IInventoryService> inventoryMock = new Mock<IInventoryService>();
        Mock<IOrderRepository> orderRepositoryMock = new Mock<IOrderRepository>();
        Mock<INotificationService> notificationMock = new Mock<INotificationService>();
        inventoryMock.Setup(x => x.IsAvailableAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        CheckoutService checkoutService = new CheckoutService(orderRepositoryMock.Object , 
            inventoryMock.Object, notificationMock.Object);
        
        
        // Act
        var result = await checkoutService.CheckoutAsync(order);
        
        
        // Assert
        result.Subtotal.Should().Be(expectedSubtotal);
        result.Discount.Should().Be(expectedDiscount);
        result.Total.Should().Be(expectedTotal);
    }
    
    
    
    //Task 2 : 2.1 --->  Valid order, all items in stock
}