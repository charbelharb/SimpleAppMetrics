namespace TestPlatform;

public interface ITestRunner : IDisposable
{
    /// <summary>
    /// Start testing
    /// </summary>
    /// <returns>Tests Result</returns>
    /// <exception cref="Exception">Will throw exception if not handled in tests</exception>
    IList<ITestResult> Start();

    /// <summary>
    /// Start Testing Asynchronously
    /// </summary>
    /// <returns>Tests Result</returns>
    /// <exception cref="Exception">Will throw exception if not handled in tests</exception>
    Task<IList<ITestResult>> StartAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Start testing
    /// </summary>
    /// <returns>Tests Result</returns>
    /// <exception cref="Exception">Will throw exception if not handled in tests</exception>
    IList<ITestResult> SafeStart();

    /// <summary>
    /// Start Testing Asynchronously
    /// </summary>
    /// <returns>Tests Result</returns>
    /// <exception cref="Exception">Will throw exception if not handled in tests</exception>
    Task<IList<ITestResult>> SafeStartAsync(CancellationToken cancellationToken = default);
}