using Microsoft.Extensions.Diagnostics.HealthChecks;
using SimpleAppMetrics.Extensions;

namespace SimpleAppMetrics;

/// <summary>
/// Health check that runs test runner and reports health status
/// </summary>
public class TestRunnerHealthCheck : IHealthCheck
{
    private readonly ITestRunner _testRunner;

    /// <summary>
    /// Creates a new instance of TestRunnerHealthCheck
    /// </summary>
    /// <param name="testRunner">The test runner to execute</param>
    public TestRunnerHealthCheck(ITestRunner testRunner)
    {
        _testRunner = testRunner ?? throw new ArgumentNullException(nameof(testRunner));
    }

    /// <summary>
    /// Runs health check by executing all tests
    /// </summary>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var results = await _testRunner.SafeStartAsync(cancellationToken);

            // Check for fatal errors first
            var fatalResults = results.Where(r => r.IsFatal()).ToList();
            if (fatalResults.Any())
            {
                return HealthCheckResult.Unhealthy(
                    description: "Fatal test failures detected",
                    data: CreateHealthData(results, fatalResults));
            }

            // Check for failures
            var failedResults = results.Where(r => r.IsFailure()).ToList();
            if (failedResults.Any())
            {
                return HealthCheckResult.Unhealthy(
                    description: "Test failures detected",
                    data: CreateHealthData(results, failedResults));
            }

            // Check for degraded performance
            var degradedResults = results.Where(r => r.IsDegraded()).ToList();
            if (degradedResults.Any())
            {
                return HealthCheckResult.Degraded(
                    description: "Performance degraded",
                    data: CreateHealthData(results, degradedResults));
            }

            // Check for warnings
            var warningResults = results.Where(r => r.HasWarnings()).ToList();
            if (warningResults.Any())
            {
                return HealthCheckResult.Degraded(
                    description: "Warnings detected",
                    data: CreateHealthData(results, warningResults));
            }

            // All healthy
            return HealthCheckResult.Healthy(
                description: "All tests passed",
                data: CreateHealthData(results, null));
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                description: "Health check execution failed",
                exception: ex);
        }
    }

    private static IReadOnlyDictionary<string, object> CreateHealthData(
        IList<ITestResult> allResults,
        IList<ITestResult>? problemResults)
    {
        var data = new Dictionary<string, object>
        {
            ["TotalTests"] = allResults.Count,
            ["PassedTests"] = allResults.Count(r => r.IsSuccess()),
            ["FailedTests"] = allResults.Count(r => r.IsFailure()),
            ["FatalTests"] = allResults.Count(r => r.IsFatal()),
            ["DegradedTests"] = allResults.Count(r => r.IsDegraded()),
            ["TestsWithWarnings"] = allResults.Count(r => r.HasWarnings())
        };

        if (problemResults?.Any() == true)
        {
            data["ProblemTests"] = problemResults.Select(r => r.GetSummary()).ToArray();
        }

        return data;
    }
}