using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;

namespace SimpleAppMetrics.UnitTests;

public class DiConfigurationTests
{
    [Fact]
    public void AddDefaultTestRunner_WithTestResultHelper_RegistersAllServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDefaultTestRunner()
            .WithTestResultHelper()
            .Build();

        var provider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(provider.GetService<ITestRunner>());
        Assert.NotNull(provider.GetService<TestResultHelper>());
        Assert.NotNull(provider.GetService<TimeProvider>());
    }

    [Fact]
    public void AddDefaultTestRunner_WithCustomTimeProvider_UsesProvidedInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        var fakeTimeProvider = new FakeTimeProvider();

        // Act
        services.AddDefaultTestRunner()
            .WithTestResultHelper(fakeTimeProvider)
            .Build();

        var provider = services.BuildServiceProvider();

        // Assert
        var registeredTimeProvider = provider.GetService<TimeProvider>();
        Assert.Same(fakeTimeProvider, registeredTimeProvider);
    }

    [Fact]
    public void AddDefaultTestRunner_WithOptions_ConfiguresCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDefaultTestRunner(options =>
        {
            options.UseTestResultHelper = true;
        });

        var provider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(provider.GetService<TestResultHelper>());
    }
}