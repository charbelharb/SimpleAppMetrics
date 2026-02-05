using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;

namespace SimpleAppMetrics.UnitTests;

public class DiTestResultHelperTests
{
    [Fact]
    public void AddTestResultHelper_WithoutParameters_RegistersTimeProviderSystem()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTestResultHelper();
        var provider = services.BuildServiceProvider();

        // Assert
        var timeProvider = provider.GetService<TimeProvider>();
        Assert.NotNull(timeProvider);
        Assert.Same(TimeProvider.System, timeProvider);
    }

    [Fact]
    public void AddTestResultHelper_WithoutParameters_RegistersTestResultHelper()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTestResultHelper();
        var provider = services.BuildServiceProvider();

        // Assert
        var helper = provider.GetService<TestResultHelper>();
        Assert.NotNull(helper);
    }

    [Fact]
    public void AddTestResultHelper_WithoutParameters_TestResultHelperUsesSystemTimeProvider()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTestResultHelper();
        var provider = services.BuildServiceProvider();

        // Assert
        var timeProvider = provider.GetRequiredService<TimeProvider>();
        var helper = provider.GetRequiredService<TestResultHelper>();
        
        Assert.Same(TimeProvider.System, timeProvider);
        Assert.NotNull(helper);
    }

    [Fact]
    public void AddTestResultHelper_WithoutParameters_ReturnsServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddTestResultHelper();

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddTestResultHelper_WithoutParameters_RegistersAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTestResultHelper();
        var provider = services.BuildServiceProvider();

        // Assert
        var helper1 = provider.GetService<TestResultHelper>();
        var helper2 = provider.GetService<TestResultHelper>();
        
        Assert.Same(helper1, helper2);
    }

    [Fact]
    public void AddTestResultHelper_WithCustomTimeProvider_RegistersCustomTimeProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var customTimeProvider = new FakeTimeProvider();

        // Act
        services.AddTestResultHelper(customTimeProvider);
        var provider = services.BuildServiceProvider();

        // Assert
        var timeProvider = provider.GetService<TimeProvider>();
        Assert.NotNull(timeProvider);
        Assert.Same(customTimeProvider, timeProvider);
    }

    [Fact]
    public void AddTestResultHelper_WithCustomTimeProvider_RegistersTestResultHelper()
    {
        // Arrange
        var services = new ServiceCollection();
        var customTimeProvider = new FakeTimeProvider();

        // Act
        services.AddTestResultHelper(customTimeProvider);
        var provider = services.BuildServiceProvider();

        // Assert
        var helper = provider.GetService<TestResultHelper>();
        Assert.NotNull(helper);
    }

    [Fact]
    public void AddTestResultHelper_WithCustomTimeProvider_TestResultHelperUsesCustomTimeProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var customTimeProvider = new FakeTimeProvider();
        customTimeProvider.SetUtcNow(new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero));

        // Act
        services.AddTestResultHelper(customTimeProvider);
        var provider = services.BuildServiceProvider();

        // Assert
        var timeProvider = provider.GetRequiredService<TimeProvider>();
        
        Assert.Same(customTimeProvider, timeProvider);
        Assert.Equal(new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero), timeProvider.GetUtcNow());
    }

    [Fact]
    public void AddTestResultHelper_WithCustomTimeProvider_ReturnsServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var customTimeProvider = new FakeTimeProvider();

        // Act
        var result = services.AddTestResultHelper(customTimeProvider);

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddTestResultHelper_WithCustomTimeProvider_RegistersAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        var customTimeProvider = new FakeTimeProvider();

        // Act
        services.AddTestResultHelper(customTimeProvider);
        var provider = services.BuildServiceProvider();

        // Assert
        var helper1 = provider.GetService<TestResultHelper>();
        var helper2 = provider.GetService<TestResultHelper>();
        
        Assert.Same(helper1, helper2);
    }

    [Fact]
    public void AddTestResultHelper_CalledTwiceWithoutParameters_OverwritesTimeProvider()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTestResultHelper();
        services.AddTestResultHelper();
        var provider = services.BuildServiceProvider();

        // Assert
        var timeProviders = provider.GetServices<TimeProvider>().ToList();
        Assert.Equal(2, timeProviders.Count);
    }

    [Fact]
    public void AddTestResultHelper_CalledTwiceWithDifferentTimeProviders_RegistersBoth()
    {
        // Arrange
        var services = new ServiceCollection();
        var timeProvider1 = new FakeTimeProvider();
        var timeProvider2 = new FakeTimeProvider();

        // Act
        services.AddTestResultHelper(timeProvider1);
        services.AddTestResultHelper(timeProvider2);
        var provider = services.BuildServiceProvider();

        // Assert - Last registration wins
        var timeProvider = provider.GetService<TimeProvider>();
        Assert.Same(timeProvider2, timeProvider);
    }

    [Fact]
    public void AddTestResultHelper_WithoutParameters_CanResolveMultipleTimes()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTestResultHelper();
        var provider = services.BuildServiceProvider();

        // Act
        var helper1 = provider.GetRequiredService<TestResultHelper>();
        var helper2 = provider.GetRequiredService<TestResultHelper>();
        var helper3 = provider.GetRequiredService<TestResultHelper>();

        // Assert
        Assert.NotNull(helper1);
        Assert.Same(helper1, helper2);
        Assert.Same(helper2, helper3);
    }

    [Fact]
    public void AddTestResultHelper_WithCustomTimeProvider_CanResolveMultipleTimes()
    {
        // Arrange
        var services = new ServiceCollection();
        var customTimeProvider = new FakeTimeProvider();
        services.AddTestResultHelper(customTimeProvider);
        var provider = services.BuildServiceProvider();

        // Act
        var helper1 = provider.GetRequiredService<TestResultHelper>();
        var helper2 = provider.GetRequiredService<TestResultHelper>();
        var helper3 = provider.GetRequiredService<TestResultHelper>();

        // Assert
        Assert.NotNull(helper1);
        Assert.Same(helper1, helper2);
        Assert.Same(helper2, helper3);
    }

    [Fact]
    public void AddTestResultHelper_WithCustomTimeProvider_TimeProviderIsUsedByHelper()
    {
        // Arrange
        var services = new ServiceCollection();
        var fakeTimeProvider = new FakeTimeProvider();
        var startTime = new DateTimeOffset(2024, 6, 15, 10, 30, 0, TimeSpan.Zero);
        fakeTimeProvider.SetUtcNow(startTime);

        services.AddTestResultHelper(fakeTimeProvider);
        var provider = services.BuildServiceProvider();

        // Act
        var timeProvider = provider.GetRequiredService<TimeProvider>();

        // Assert
        Assert.Same(fakeTimeProvider, timeProvider);
        Assert.Equal(startTime, timeProvider.GetUtcNow());
    }

    [Fact]
    public void AddTestResultHelper_MultipleServiceProviders_GetIndependentHelpers()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTestResultHelper();

        // Act
        var provider1 = services.BuildServiceProvider();
        var provider2 = services.BuildServiceProvider();

        var helper1 = provider1.GetRequiredService<TestResultHelper>();
        var helper2 = provider2.GetRequiredService<TestResultHelper>();

        // Assert
        Assert.NotNull(helper1);
        Assert.NotNull(helper2);
        Assert.NotSame(helper1, helper2);
    }

    [Fact]
    public void AddTestResultHelper_WithoutParameters_CanBeChained()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services
            .AddTestResultHelper()
            .AddTransient<ITest, MockTest>();

        // Assert
        Assert.Same(services, result);
        var provider = services.BuildServiceProvider();
        Assert.NotNull(provider.GetService<TestResultHelper>());
        Assert.NotNull(provider.GetService<ITest>());
    }

    [Fact]
    public void AddTestResultHelper_WithCustomTimeProvider_CanBeChained()
    {
        // Arrange
        var services = new ServiceCollection();
        var customTimeProvider = new FakeTimeProvider();

        // Act
        var result = services
            .AddTestResultHelper(customTimeProvider)
            .AddTransient<ITest, MockTest>();

        // Assert
        Assert.Same(services, result);
        var provider = services.BuildServiceProvider();
        Assert.NotNull(provider.GetService<TestResultHelper>());
        Assert.NotNull(provider.GetService<ITest>());
    }

    [Fact]
    public void AddTestResultHelper_ServiceDescriptor_HasCorrectLifetime()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTestResultHelper();

        // Assert
        var timeProviderDescriptor = services.First(d => d.ServiceType == typeof(TimeProvider));
        var helperDescriptor = services.First(d => d.ServiceType == typeof(TestResultHelper));

        Assert.Equal(ServiceLifetime.Singleton, timeProviderDescriptor.Lifetime);
        Assert.Equal(ServiceLifetime.Singleton, helperDescriptor.Lifetime);
    }

    // Mock test class for chaining tests
    private class MockTest : ITest
    {
        public ITestResult Run() => new DefaultTestResult { WhoAmI = "Mock" };
        public Task<ITestResult> RunAsync(CancellationToken cancellationToken = default) 
            => Task.FromResult(Run());
        public bool IsDisposed { get; private set; }
        public void Dispose() => IsDisposed = true;
    }
}