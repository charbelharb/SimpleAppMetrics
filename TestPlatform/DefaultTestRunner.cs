using Microsoft.Extensions.DependencyInjection;

namespace TestPlatform;

public class DefaultTestRunner : ITestRunner
{
    private readonly IList<ITest> _tests = new List<ITest>();
    
    private DefaultTestRunner()
    {
    }

    public DefaultTestRunner(IList<ITest> tests)
    {
        _tests = tests;
    }
    
    public DefaultTestRunner(IServiceProvider serviceProvider)
    {
        _tests = serviceProvider.GetServices<ITest>().ToList();
    }
    
    public IList<ITestResult> Start()
    {
        return _tests.Select(test => test.Run()).ToList();
    }

    public async Task<IList<ITestResult>> StartAsync(CancellationToken cancellationToken = default)
    {
        var result = await Task.WhenAll(_tests.Select(test => test.RunAsync(cancellationToken)));
        return result.ToList();
    }

    public IList<ITestResult> SafeStart()
    {
        var result = new List<ITestResult>();
        foreach (var test in _tests)
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
                    Exceptions = new List<Exception> {e},
                    WhoAmI = test.GetType().Name
                });
            }
        }
        return result;
    }

    public async Task<IList<ITestResult>> SafeStartAsync(CancellationToken cancellationToken = default)
    {
        var result = new List<ITestResult>();
        foreach (var test in _tests)
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
                    Exceptions = new List<Exception> {e},
                    WhoAmI = test.GetType().Name
                });
            }
        }
        return result;
    }

    public void Dispose()
    {
        foreach (var test in _tests)
        {
            test.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}