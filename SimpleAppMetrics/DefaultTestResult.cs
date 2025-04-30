using System.Diagnostics.CodeAnalysis;

namespace SimpleAppMetrics;

[ExcludeFromCodeCoverage]
public class DefaultTestResult : ITestResult
{
    public TestResultStatus Status { get; set; } = TestResultStatus.Pass;
    public IList<string> SuccessMessages { get; set; } = new List<string>();
    public IList<string> DegradedMessages { get; set; } = new List<string>();
    public IList<string> Errors { get; set; } = new List<string>();
    public IList<string> Warnings { get; set; } = new List<string>();
    public IList<Exception> Exceptions { get; set; } = new List<Exception>();
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public required string WhoAmI { get; set; }
    public object? Data { get; set; }
}