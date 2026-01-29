namespace SimpleAppMetrics;

public class DefaultTestRunner(IEnumerable<ITest> tests) : ITestRunner
{
    /// <inheritdoc/>
    public IList<ITestResult> Start()
    {
        LastRunResults = tests.Select(test => test.Run()).ToList();
        return LastRunResults;
    }

    /// <inheritdoc/>
    public async Task<IList<ITestResult>> StartAsync(CancellationToken cancellationToken = default)
    {
        LastRunResults = (await Task.WhenAll(tests.Select(test => test.RunAsync(cancellationToken)))).ToList();
        return LastRunResults;
    }

    /// <inheritdoc/>
    public IList<ITestResult> SafeStart()
    {
        var result = new List<ITestResult>();
        foreach (var test in tests)
        {
            try
            {
                var testResult = test.Run();
                result.Add(testResult);
            }
            catch (Exception e)
            {
                result.Add(new DefaultTestResult
                {
                    Status = TestResultStatus.Fatal,
                    ExceptionObjects =  new List<Exception> { e },
                    Exceptions = new List<string>{ e.ToString() },
                    WhoAmI = test.GetType().Name
                });
            }
        }
        LastRunResults = result;
        return LastRunResults;
    }

    /// <inheritdoc/>
    public async Task<IList<ITestResult>> SafeStartAsync(CancellationToken cancellationToken = default)
    {
        var tasks = tests.Select(async test =>
        {
            try
            {
                return await test.RunAsync(cancellationToken);
            }
            catch (Exception e)
            {
                return new DefaultTestResult
                {
                    Status = TestResultStatus.Fatal,
                    ExceptionObjects =  new List<Exception> { e },
                    Exceptions = new List<string> { e.ToString() },
                    WhoAmI = test.GetType().Name
                };
            }
        });
        LastRunResults = (await Task.WhenAll(tasks)).ToList();
        return LastRunResults;
    }

    /// <inheritdoc/>
    public IList<ITestResult> LastRunResults { get; private set; } = new List<ITestResult>();
    
    /// <inheritdoc/>
    public bool IsDisposed { get; private set; }
    
    public void Dispose()
    {
        foreach (var test in tests)
        {
            test.Dispose();
        }
        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
}