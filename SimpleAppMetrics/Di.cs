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
    /// This will register DefaultTestRunner as Transient
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/></param>
    public static void AddDefaultTestRunner(this IServiceCollection services)
    {
        services.AddTransient<ITestRunner, DefaultTestRunner>();
    }
}