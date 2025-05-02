using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace SimpleAppMetrics;

[ExcludeFromCodeCoverage]
public class DefaultTestResult : ITestResult
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TestResultStatus Status { get; set; } = TestResultStatus.Pass;
    public TestResultStatus StatusCode => Status;
    public IList<string> SuccessMessages { get; set; } = new List<string>();
    public IList<string> DegradedMessages { get; set; } = new List<string>();
    public IList<string> Errors { get; set; } = new List<string>();
    public IList<string> Warnings { get; set; } = new List<string>();
    public IList<string> Exceptions { get; set; } = new List<string>();
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public TimeSpan? Elapsed { get; set; }
    public int? ElapsedMilliseconds => Elapsed?.Milliseconds;
    public required string WhoAmI { get; set; }
    public object? Data { get; set; }
}