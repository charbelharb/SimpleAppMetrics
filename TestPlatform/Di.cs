using Microsoft.Extensions.DependencyInjection;

namespace TestPlatform;

public static class Di
{
    public static void AddTestRunner(this IServiceCollection services)
    {
        services.AddTransient<ITestRunner, DefaultTestRunner>();
    }
}