namespace TestPlatform;

public interface ITest : IDisposable
{
    /// <summary>
    /// Run the test
    /// </summary>
    /// <returns></returns>
    ITestResult Run();
    
    /// <summary>
    /// Run Asynchronously
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ITestResult> RunAsync(CancellationToken cancellationToken = default);
}