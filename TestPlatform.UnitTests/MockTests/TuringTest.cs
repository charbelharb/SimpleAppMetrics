using System.Diagnostics.CodeAnalysis;

namespace TestPlatform.UnitTests.MockTests;

[ExcludeFromCodeCoverage]
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
        return await Task.FromResult(new DefaultTestResult
        {
            WhoAmI = nameof(TuringTest)
        });
    }

    public bool IsDisposed { get; private set; }

    public void Dispose()
    {
        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
}