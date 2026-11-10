using NetArchTest.Rules;
using System.Reflection;
using FluentAssertions;
using Simulation.Checkout.Application;
using Simulation.Checkout.Application.Abstractions;
using Simulation.Checkout.Domain;
using Simulation.Checkout.Infrastructure;

namespace Simulation.Checkout.Tests;

public class ArchitectureTests
{
    
    // Task 5 : 5.1 -  Domain does not depend on Application or Infrastructure.
    [Fact]
    public void Domain_WhenCheckingDependencies_ShouldNotDependOnApplicationOrInfrastructure()
    {
        // Arrange
        Assembly domainAssembly = typeof(Order).Assembly;

        
        // Act
        var result = Types.InAssembly(domainAssembly)
            .ShouldNot()
            .HaveDependencyOn("Simulation.Checkout.Application")
            .And()
            .NotHaveDependencyOn("Simulation.Checkout.Infrastructure")
            .GetResult();
        

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
    
    
    // Task 5 : 5.2 -   Application does not depend on Infrastructure.
    [Fact]
    public void Application_WhenCheckingDependencies_ShouldNotDependOnInfrastructure()
    {
        // Arrange
        Assembly applicationAssembly = typeof(CheckoutService).Assembly;

        
        // Act
        var result = Types.InAssembly(applicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("Simulation.Checkout.Infrastructure")
            .GetResult();
        

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
    
    
    //Task 5 : 5.3 -  All interfaces start with I
    [Fact]
    public void Interfaces_WhenCheckingNames_ShouldStartWithI()
    {
        // Arrange
        Assembly applicationAssembly = typeof(CheckoutService).Assembly;
        Assembly domainAssembly = typeof(Order).Assembly;
        Assembly infrastructureAssembly = typeof(InMemoryOrderRepository).Assembly;
        
        // Act
        var result = Types.InAssemblies([applicationAssembly, domainAssembly, infrastructureAssembly])
            .That()
            .AreInterfaces()
            .Should()
            .HaveNameStartingWith("I")
            .GetResult();
        

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
    
    
    // Task 5 : 5.4 -   Classes that implement IOrderRepository end with Repository.
    [Fact]
    public void OrderRepositoryImplementations_WhenCheckingNames_ShouldEndWithRepository()
    {
        // Arrange
        Assembly applicationAssembly = typeof(CheckoutService).Assembly;
        Assembly domainAssembly = typeof(Order).Assembly;
        Assembly infrastructureAssembly = typeof(InMemoryOrderRepository).Assembly;
        
        // Act
        var result = Types.InAssemblies([applicationAssembly, domainAssembly, infrastructureAssembly])
            .That()
            .AreClasses()
            .And()
            .ImplementInterface(typeof(IOrderRepository))
            .Should()
            .HaveNameEndingWith("Repository")
            .GetResult();
        

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
}