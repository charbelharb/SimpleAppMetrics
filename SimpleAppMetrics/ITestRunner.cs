namespace SimpleAppMetrics;

public interface ITestRunner : IDisposable
{
    /// <summary>
    /// Start testing
    /// </summary>
    /// <returns>An <see cref="IList{T}"/> of<see cref="ITestResult"/></returns>
    /// <exception cref="Exception">Will throw exception if not handled in tests</exception>
    IList<ITestResult> Start();

    /// <summary>
    /// Start Testing Asynchronously
    /// </summary>
    /// <returns>A <see cref="Task"/>of an <see cref="IList{T}"/> of<see cref="ITestResult"/></returns>
    /// <exception cref="Exception">Will throw exception if not handled in tests</exception>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    Task<IList<ITestResult>> StartAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Start testing
    /// </summary>
    /// <returns>An <see cref="IList{T}"/> of<see cref="ITestResult"/></returns>
    /// <exception cref="Exception">Will not throw tests exceptions</exception>
    IList<ITestResult> SafeStart();

    /// <summary>
    /// Start Testing Asynchronously
    /// </summary>
    /// <returns>A <see cref="Task"/> of an <see cref="IList{T}"/> of<see cref="ITestResult"/></returns>
    /// <exception cref="Exception">Will not throw tests exceptions</exception>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    Task<IList<ITestResult>> SafeStartAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// List of last run results
    /// </summary>
    IList<ITestResult> LastRunResults { get; }
    
    /// <summary>
    /// Check if the test runner is disposed
    /// </summary>
    bool IsDisposed { get; }
}