namespace SimpleAppMetrics;

/// <summary>
/// Extension methods for ITestResult
/// </summary>
public static class TestResultExtensions
{
    /// <summary>
    /// Checks if the test result indicates success
    /// </summary>
    public static bool IsSuccess(this ITestResult result) 
        => result.Status.HasFlag(TestResultStatus.Pass);
    
    /// <summary>
    /// Checks if the test result indicates failure
    /// </summary>
    public static bool IsFailure(this ITestResult result)
        => result.Status.HasFlag(TestResultStatus.Fail);
    
    /// <summary>
    /// Checks if the test result indicates a fatal error
    /// </summary>
    public static bool IsFatal(this ITestResult result)
        => result.Status.HasFlag(TestResultStatus.Fatal);
    
    /// <summary>
    /// Checks if the test result has warnings
    /// </summary>
    public static bool HasWarnings(this ITestResult result)
        => result.Status.HasFlag(TestResultStatus.Warning);
    
    /// <summary>
    /// Checks if the test result has degraded performance
    /// </summary>
    public static bool IsDegraded(this ITestResult result)
        => result.Status.HasFlag(TestResultStatus.Degraded);
    
    /// <summary>
    /// Checks if the test result has errors
    /// </summary>
    public static bool HasErrors(this ITestResult result)
        => result.Status.HasFlag(TestResultStatus.Errors);
    
    /// <summary>
    /// Checks if the test result has exceptions
    /// </summary>
    public static bool HasExceptions(this ITestResult result)
        => result.Status.HasFlag(TestResultStatus.Exceptions);
    
    /// <summary>
    /// Gets a summary string of the test result
    /// </summary>
    public static string GetSummary(this ITestResult result)
        => $"{result.WhoAmI}: {result.Status} ({result.ElapsedMilliseconds ?? 0}ms)";
    
    /// <summary>
    /// Gets a detailed summary including all messages
    /// </summary>
    public static string GetDetailedSummary(this ITestResult result)
    {
        var summary = new System.Text.StringBuilder();
        summary.AppendLine($"Test: {result.WhoAmI}");
        summary.AppendLine($"Status: {result.Status}");
        summary.AppendLine($"Duration: {result.ElapsedMilliseconds ?? 0}ms");
        
        if (result.SuccessMessages.Any())
            summary.AppendLine($"Success: {string.Join(", ", result.SuccessMessages)}");
        
        if (result.Warnings.Any())
            summary.AppendLine($"Warnings: {string.Join(", ", result.Warnings)}");
        
        if (result.Errors.Any())
            summary.AppendLine($"Errors: {string.Join(", ", result.Errors)}");
        
        if (result.Exceptions.Any())
            summary.AppendLine($"Exceptions: {string.Join(", ", result.Exceptions)}");
        
        if (result.DegradedMessages.Any())
            summary.AppendLine($"Degraded: {string.Join(", ", result.DegradedMessages)}");
        
        return summary.ToString();
    }
    
    /// <summary>
    /// Checks if the result is completely healthy (Pass with no issues)
    /// </summary>
    public static bool IsHealthy(this ITestResult result)
        => result.Status == TestResultStatus.Pass;
    
    /// <summary>
    /// Checks if the result passes but with some issues
    /// </summary>
    public static bool IsPassingWithIssues(this ITestResult result)
        => result.IsSuccess() && result.Status != TestResultStatus.Pass;
    
    /// <summary>
    /// Gets all issue messages combined
    /// </summary>
    public static IEnumerable<string> GetAllIssues(this ITestResult result)
    {
        return result.Warnings
            .Concat(result.Errors)
            .Concat(result.Exceptions)
            .Concat(result.DegradedMessages);
    }
}