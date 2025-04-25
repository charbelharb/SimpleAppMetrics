using TestPlatform;

namespace HealthCheck;

public class TheHatedTest : ITest
{
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public ITestResult Run()
    {
        return new DefaultTestResult()
        {
            WhoAmI = nameof(TheHatedTest),
            Status = TestResultStatus.FailWithAllIssues
        };
    }

    public async Task<ITestResult> RunAsync(CancellationToken cancellationToken = default)
    {
        return new DefaultTestResult()
        {
            WhoAmI = nameof(TheHatedTest),
            Status = TestResultStatus.FailWithAllIssues
        };
    }
}