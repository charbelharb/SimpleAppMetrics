namespace SimpleAppMetrics;

public interface ITest : IDisposable
{
    /// <summary>
    /// Run the test
    /// </summary>
    /// <returns>An <see cref="ITestResult"/></returns>
    ITestResult Run();
    
    /// <summary>
    /// Run Asynchronously
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="Task"/> of <see cref="ITestResult"/></returns>
    Task<ITestResult> RunAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if the test is disposed
    /// </summary>
    bool IsDisposed { get; }
}