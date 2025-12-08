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
    
    public static void AddDefaultTestRunner(this IServiceCollection services)
    {
        services.AddTransient<ITestRunner, DefaultTestRunner>();
    }
}