using Microsoft.Extensions.DependencyInjection;

namespace SimpleAppMetrics.UnitTests;

public class SimpleAppMetricsDiTests
{
    [Fact]
    public void Di_AddDefaultTestRunner_ShouldRegisterDefaultTestRunner()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDefaultTestRunner();
        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        
        // Act
        var defaultRunner = serviceProvider.GetService<ITestRunner>();

        // Assert
        Assert.NotNull(defaultRunner);
        Assert.IsType<DefaultTestRunner>(defaultRunner);
    }
}