using System.Diagnostics.CodeAnalysis;

namespace SimpleAppMetrics;

/// <summary>
/// Helper utilities for creating and timing test results
/// </summary>
public class TestResultHelper
{
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Creates a new instance with the specified time provider
    /// </summary>
    /// <param name="timeProvider">Time provider for getting current time (defaults to system time)</param>
    public TestResultHelper(TimeProvider? timeProvider = null)
    {
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    /// <summary>
    /// Creates a result with autopopulated WhoAmI from test type
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static DefaultTestResult CreateFor<T>() where T : ITest
    {
        return new DefaultTestResult { WhoAmI = typeof(T).Name };
    }
    
    /// <summary>
    /// Creates a result with autopopulated WhoAmI from test instance
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static DefaultTestResult CreateFor(ITest test)
    {
        return new DefaultTestResult { WhoAmI = test.GetType().Name };
    }
    
    /// <summary>
    /// Wraps test execution with automatic timing
    /// </summary>
    public ITestResult RunWithTiming(ITest test)
    {
        var startDate = _timeProvider.GetUtcNow().DateTime;
        var result = test.Run();
        result.StartDate ??= startDate;
        result.EndDate ??= _timeProvider.GetUtcNow().DateTime;
        result.Elapsed ??= result.EndDate - result.StartDate;
        return result;
    }
    
    /// <summary>
    /// Wraps async test execution with automatic timing
    /// </summary>
    public async Task<ITestResult> RunWithTimingAsync(
        ITest test, 
        CancellationToken cancellationToken = default)
    {
        var startDate = _timeProvider.GetUtcNow();
        var result = await test.RunAsync(cancellationToken);
        result.StartDate ??= startDate.DateTime;
        result.EndDate ??= _timeProvider.GetUtcNow().DateTime;
        result.Elapsed ??= result.EndDate - result.StartDate;
        return result;
    }

    /// <summary>
    /// Measures execution time and returns both result and elapsed time
    /// </summary>
    public (ITestResult Result, TimeSpan Elapsed) MeasureExecution(ITest test)
    {
        var startTimestamp = _timeProvider.GetTimestamp();
        var result = test.Run();
        var elapsed = _timeProvider.GetElapsedTime(startTimestamp);
        
        result.Elapsed ??= elapsed;
        return (result, elapsed);
    }

    /// <summary>
    /// Measures async execution time and returns both result and elapsed time
    /// </summary>
    public async Task<(ITestResult Result, TimeSpan Elapsed)> MeasureExecutionAsync(
        ITest test,
        CancellationToken cancellationToken = default)
    {
        var startTimestamp = _timeProvider.GetTimestamp();
        var result = await test.RunAsync(cancellationToken);
        var elapsed = _timeProvider.GetElapsedTime(startTimestamp);
        
        result.Elapsed ??= elapsed;
        return (result, elapsed);
    }
}