using System.Diagnostics.CodeAnalysis;

namespace TestPlatform.UnitTests.MockTests;

[ExcludeFromCodeCoverage]
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
        return await Task.FromResult(new DefaultTestResult
        {
            WhoAmI = nameof(FooBarTest),
            Status = TestResultStatus.PassWithDegraded,
            DegradedMessages = new List<string>
            {
                "Dead"
            },
        });
    }

    public bool IsDisposed { get; private set; }

    public void Dispose()
    {
        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
}