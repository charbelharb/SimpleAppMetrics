﻿using Microsoft.Extensions.DependencyInjection;

namespace SimpleAppMetrics.UnitTests;

public class SimpleAppMetricsDiTests
{
    [Fact]
    public void Di_AddTestRunner_ShouldRegisterDefaultTestRunner()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTestRunner();
        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        
        // Act
        var defaultRunner = serviceProvider.GetService<ITestRunner>();

        // Assert
        Assert.NotNull(defaultRunner);
        Assert.IsType<DefaultTestRunner>(defaultRunner);
    }
}