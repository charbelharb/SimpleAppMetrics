using TestPlatform;

namespace HealthCheck;

public class FooBarTest : ITest
{
    public ITestResult Run()
    {
        return new DefaultTestResult
        {
            WhoAmI = nameof(FooBarTest)
        };
    }

    public async Task<ITestResult> RunAsync(CancellationToken cancellationToken = default)
    {
        return new DefaultTestResult
        {
            WhoAmI = nameof(FooBarTest),
            Status = TestResultStatus.PassWithDegraded,
            DegradedMessages = new List<string>
            {
                "Dead"
            },
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}