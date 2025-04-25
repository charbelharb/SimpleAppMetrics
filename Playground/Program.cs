
using HealthCheck;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using TestPlatform;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTransient<ITest, TuringTest>();
builder.Services.AddTransient<ITest, FooBarTest>();
builder.Services.AddTestRunner();
// TheHatedTest is not registered in the DI container, ergo, it will not be run
var host = builder.Build();
RunTests(host.Services);
return;


static void RunTests(IServiceProvider hostProvider)
{
    using var serviceScope = hostProvider.CreateScope();
    var provider = serviceScope.ServiceProvider;
    var testRunner = provider.GetRequiredService<ITestRunner>();
    var results = testRunner.Start();
    foreach (var result in results)
    {
        Console.WriteLine($"Running test {result.WhoAmI}");
    }
}


