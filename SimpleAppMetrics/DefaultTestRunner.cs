using Microsoft.Extensions.DependencyInjection;

namespace SimpleAppMetrics;

public class DefaultTestRunner(IList<ITest> tests) : ITestRunner
{
    public DefaultTestRunner(IServiceProvider serviceProvider) : this(serviceProvider.GetServices<ITest>().ToList())
    {
    }
    
    public IList<ITestResult> Start()
    {
        LastRunResults = tests.Select(test => test.Run()).ToList();
        return LastRunResults;
    }

    public async Task<IList<ITestResult>> StartAsync(CancellationToken cancellationToken = default)
    {
        LastRunResults = (await Task.WhenAll(tests.Select(test => test.RunAsync(cancellationToken)))).ToList();
        return LastRunResults;
    }

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
                    Exceptions = new List<string>{ e.ToString() },
                    WhoAmI = test.GetType().Name
                });
            }
        }
        LastRunResults = result;
        return LastRunResults;
    }

    public async Task<IList<ITestResult>> SafeStartAsync(CancellationToken cancellationToken = default)
    {
        var result = new List<ITestResult>();
        foreach (var test in tests)
        {
            try
            {
                var testResult = await test.RunAsync(cancellationToken);
                result.Add(testResult);
            }
            catch (Exception e)
            {
                result.Add(new DefaultTestResult
                {
                    Status = TestResultStatus.Fatal,
                    Exceptions = new List<string> { e.ToString() },
                    WhoAmI = test.GetType().Name
                });
            }
        }
        LastRunResults = result;
        return LastRunResults;
    }

    public IList<ITestResult> LastRunResults { get; private set; } = new List<ITestResult>();
    
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