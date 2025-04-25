using TestPlatform;

namespace HealthCheck;

public class TuringTest : ITest
{
    public ITestResult Run()
    {
        return new DefaultTestResult
        {
            WhoAmI = nameof(TuringTest)
        };
    }

    public async Task<ITestResult> RunAsync(CancellationToken cancellationToken = default)
    {
        return new DefaultTestResult
        {
            WhoAmI = nameof(TuringTest)
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}