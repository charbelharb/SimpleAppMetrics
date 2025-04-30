using System.Diagnostics.CodeAnalysis;

namespace SimpleAppMetrics.UnitTests.MockTests;

[ExcludeFromCodeCoverage]
public class TheAlwaysThrowingExceptionTest : ITest
{
    public ITestResult Run()
    {
        throw new Exception("Not today");
    }

    public async Task<ITestResult> RunAsync(CancellationToken cancellationToken = default)
    {
       throw new Exception("Not today");
    }
    
    public bool IsDisposed { get; private set; }

    public void Dispose()
    {
        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
}