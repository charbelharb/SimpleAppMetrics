using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SimpleAppMetrics.Extensions;

namespace SimpleAppMetrics;

public class DefaultTestRunner(IEnumerable<ITest> tests, ILogger<DefaultTestRunner>? logger = null)
    : ITestRunner
{
    private static readonly ActivitySource ActivitySource = new("SimpleAppMetrics", Version.Current);

    /// <inheritdoc/>
    public IList<ITestResult> Start()
    {
        using var activity = ActivitySource.StartActivity();
        logger?.LogInformation("Starting test execution for {TestCount} tests", tests.Count());
        
        var stopwatch = Stopwatch.StartNew();
        LastRunResults = tests.Select(ExecuteTest).ToList();
        stopwatch.Stop();
        
        LogTestRunSummary(LastRunResults, stopwatch.Elapsed);
        activity?.SetTag("test.count", LastRunResults.Count);
        activity?.SetTag("test.duration_ms", stopwatch.ElapsedMilliseconds);
        
        return LastRunResults;
    }

    /// <inheritdoc/>
    public async Task<IList<ITestResult>> StartAsync(CancellationToken cancellationToken = default)
    {
        using var activity = ActivitySource.StartActivity();
        logger?.LogInformation("Starting async test execution for {TestCount} tests", tests.Count());
        
        var stopwatch = Stopwatch.StartNew();
        LastRunResults = (await Task.WhenAll(tests.Select(test => ExecuteTestAsync(test, cancellationToken)))).ToList();
        stopwatch.Stop();
        
        LogTestRunSummary(LastRunResults, stopwatch.Elapsed);
        activity?.SetTag("test.count", LastRunResults.Count);
        activity?.SetTag("test.duration_ms", stopwatch.ElapsedMilliseconds);
        
        return LastRunResults;
    }

    /// <inheritdoc/>
    public IList<ITestResult> SafeStart()
    {
        using var activity = ActivitySource.StartActivity();
        logger?.LogInformation("Starting safe test execution for {TestCount} tests", tests.Count());
        
        var result = new List<ITestResult>();
        var stopwatch = Stopwatch.StartNew();
        
        foreach (var test in tests)
        {
            try
            {
                var testResult = ExecuteTest(test);
                result.Add(testResult);
            }
            catch (Exception e)
            {
                var fatalResult = CreateFatalResult(test, e);
                result.Add(fatalResult);
                logger?.LogError(e, "Fatal error executing test {TestName}", test.GetType().Name);
            }
        }
        
        stopwatch.Stop();
        LastRunResults = result;
        
        LogTestRunSummary(LastRunResults, stopwatch.Elapsed);
        activity?.SetTag("test.count", LastRunResults.Count);
        activity?.SetTag("test.duration_ms", stopwatch.ElapsedMilliseconds);
        
        return LastRunResults;
    }

    /// <inheritdoc/>
    public async Task<IList<ITestResult>> SafeStartAsync(CancellationToken cancellationToken = default)
    {
        using var activity = ActivitySource.StartActivity();
        logger?.LogInformation("Starting safe async test execution for {TestCount} tests", tests.Count());
        
        var stopwatch = Stopwatch.StartNew();
        
        var tasks = tests.Select(async test =>
        {
            try
            {
                return await ExecuteTestAsync(test, cancellationToken);
            }
            catch (Exception e)
            {
                var fatalResult = CreateFatalResult(test, e);
                logger?.LogError(e, "Fatal error executing test {TestName}", test.GetType().Name);
                return fatalResult;
            }
        });
        
        LastRunResults = (await Task.WhenAll(tasks)).ToList();
        stopwatch.Stop();
        
        LogTestRunSummary(LastRunResults, stopwatch.Elapsed);
        activity?.SetTag("test.count", LastRunResults.Count);
        activity?.SetTag("test.duration_ms", stopwatch.ElapsedMilliseconds);
        
        return LastRunResults;
    }

    private ITestResult ExecuteTest(ITest test)
    {
        var testName = test.GetType().Name;
        using var activity = ActivitySource.StartActivity($"Test.{testName}");
        
        logger?.LogDebug("Executing test: {TestName}", testName);
        
        var stopwatch = Stopwatch.StartNew();
        var result = test.Run();
        stopwatch.Stop();
        
        activity?.SetTag("test.name", testName);
        activity?.SetTag("test.status", result.Status.ToString());
        activity?.SetTag("test.duration_ms", stopwatch.ElapsedMilliseconds);
        
        LogTestResult(result);
        
        return result;
    }

    private async Task<ITestResult> ExecuteTestAsync(ITest test, CancellationToken cancellationToken)
    {
        var testName = test.GetType().Name;
        using var activity = ActivitySource.StartActivity($"Test.{testName}");
        
        logger?.LogDebug("Executing async test: {TestName}", testName);
        
        var stopwatch = Stopwatch.StartNew();
        var result = await test.RunAsync(cancellationToken);
        stopwatch.Stop();
        
        activity?.SetTag("test.name", testName);
        activity?.SetTag("test.status", result.Status.ToString());
        activity?.SetTag("test.duration_ms", stopwatch.ElapsedMilliseconds);
        
        LogTestResult(result);
        
        return result;
    }

    private static DefaultTestResult CreateFatalResult(ITest test, Exception exception)
    {
        return new DefaultTestResult
        {
            Status = TestResultStatus.Fatal,
            ExceptionObjects = new List<Exception> { exception },
            Exceptions = new List<string> { exception.ToString() },
            WhoAmI = test.GetType().Name
        };
    }

    private void LogTestResult(ITestResult result)
    {
        if (logger == null) return;

        var logLevel = result.Status switch
        {
            var s when s.HasFlag(TestResultStatus.Fatal) => LogLevel.Critical,
            var s when s.HasFlag(TestResultStatus.Fail) => LogLevel.Error,
            var s when s.HasFlag(TestResultStatus.Degraded) => LogLevel.Warning,
            var s when s.HasFlag(TestResultStatus.Warning) => LogLevel.Warning,
            _ => LogLevel.Information
        };

        logger.Log(logLevel, 
            "Test {TestName} completed with status {Status} in {Duration}ms",
            result.WhoAmI, result.Status, result.ElapsedMilliseconds ?? 0);

        if (result.Exceptions.Any())
        {
            foreach (var exception in result.Exceptions)
            {
                logger.LogError("Test {TestName} exception: {Exception}", result.WhoAmI, exception);
            }
        }
    }

    private void LogTestRunSummary(IList<ITestResult> results, TimeSpan totalDuration)
    {
        if (logger == null) return;

        var passed = results.Count(r => r.IsSuccess());
        var failed = results.Count(r => r.IsFailure());
        var fatal = results.Count(r => r.IsFatal());

        logger.LogInformation(
            "Test run completed: {Total} total, {Passed} passed, {Failed} failed, {Fatal} fatal in {Duration}ms",
            results.Count, passed, failed, fatal, totalDuration.TotalMilliseconds);
    }

    /// <inheritdoc/>
    public IList<ITestResult> LastRunResults { get; private set; } = new List<ITestResult>();
    
    /// <inheritdoc/>
    public bool IsDisposed { get; private set; }
    
    public void Dispose()
    {
        logger?.LogDebug("Disposing test runner and {TestCount} tests", tests.Count());
        
        foreach (var test in tests)
        {
            test.Dispose();
        }
        
        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
}