using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleAppMetrics;

public static class Di
{
    [Obsolete("Use AddDefaultTestRunner() instead")]
    [ExcludeFromCodeCoverage]
    public static void AddTestRunner(this IServiceCollection services)
    {
        services.AddTransient<ITestRunner, DefaultTestRunner>();
    }
    
    /// <summary>
    /// Registers DefaultTestRunner as Transient and returns a builder for additional configuration
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/></param>
    /// <returns>A <see cref="TestRunnerBuilder"/> for fluent configuration</returns>
    public static TestRunnerBuilder AddDefaultTestRunner(this IServiceCollection services)
    {
        services.AddTransient<ITestRunner, DefaultTestRunner>();
        return new TestRunnerBuilder(services);
    }

    /// <summary>
    /// Registers DefaultTestRunner with configuration options
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/></param>
    /// <param name="configure">Configuration delegate</param>
    public static IServiceCollection AddDefaultTestRunner(
        this IServiceCollection services, 
        Action<TestRunnerOptions> configure)
    {
        var builder = services.AddDefaultTestRunner();
        builder.WithOptions(configure);
        return builder.Build();
    }

    /// <summary>
    /// Registers TestResultHelper with TimeProvider support
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/></param>
    public static IServiceCollection AddTestResultHelper(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<TestResultHelper>();
        return services;
    }

    /// <summary>
    /// Registers TestResultHelper with custom TimeProvider
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/></param>
    /// <param name="timeProvider">Custom TimeProvider instance</param>
    public static IServiceCollection AddTestResultHelper(
        this IServiceCollection services, 
        TimeProvider timeProvider)
    {
        services.AddSingleton(timeProvider);
        services.AddSingleton<TestResultHelper>();
        return services;
    }
}

/// <summary>
/// Configuration options for test runner setup
/// </summary>
public class TestRunnerOptions
{
    /// <summary>
    /// Whether to register TestResultHelper
    /// </summary>
    public bool UseTestResultHelper { get; set; }
    
    /// <summary>
    /// Custom TimeProvider for TestResultHelper (defaults to TimeProvider.System)
    /// </summary>
    public TimeProvider? TimeProvider { get; set; }
}

/// <summary>
/// Builder for configuring test runner services
/// </summary>
public class TestRunnerBuilder
{
    private readonly IServiceCollection _services;
    private readonly TestRunnerOptions _options;

    internal TestRunnerBuilder(IServiceCollection services)
    {
        _services = services;
        _options = new TestRunnerOptions();
    }

    /// <summary>
    /// Adds TestResultHelper to the service collection
    /// </summary>
    public TestRunnerBuilder WithTestResultHelper()
    {
        _options.UseTestResultHelper = true;
        return this;
    }

    /// <summary>
    /// Adds TestResultHelper with a custom TimeProvider
    /// </summary>
    /// <param name="timeProvider">Custom TimeProvider instance</param>
    public TestRunnerBuilder WithTestResultHelper(TimeProvider timeProvider)
    {
        _options.UseTestResultHelper = true;
        _options.TimeProvider = timeProvider;
        return this;
    }

    /// <summary>
    /// Configures options using a configuration delegate
    /// </summary>
    /// <param name="configure">Configuration delegate</param>
    public TestRunnerBuilder WithOptions(Action<TestRunnerOptions> configure)
    {
        configure(_options);
        return this;
    }

    /// <summary>
    /// Builds and registers all configured services
    /// </summary>
    public IServiceCollection Build()
    {
        // Register TestResultHelper if configured
        if (!_options.UseTestResultHelper)
        {
            return _services;
        }
        var timeProvider = _options.TimeProvider ?? TimeProvider.System;
        _services.AddSingleton(timeProvider);
        _services.AddSingleton<TestResultHelper>();
        return _services;
    }

    /// <summary>
    /// Implicit conversion to IServiceCollection for seamless chaining
    /// </summary>
    public static implicit operator ServiceCollection(TestRunnerBuilder builder)
    {
        return (ServiceCollection)builder.Build();
    }
}