using Microsoft.Extensions.DependencyInjection;

namespace SimpleAppMetrics;

public static class Di
{
    public static void AddTestRunner(this IServiceCollection services)
    {
        services.AddTransient<ITestRunner, DefaultTestRunner>();
    }
}