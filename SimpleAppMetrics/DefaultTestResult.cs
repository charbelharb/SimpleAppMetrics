using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace SimpleAppMetrics;

[ExcludeFromCodeCoverage]
public class DefaultTestResult : ITestResult
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TestResultStatus Status { get; set; } = TestResultStatus.Pass;
    
    public IList<string> SuccessMessages { get; set; } = [];
    public IList<string> DegradedMessages { get; set; } = [];
    public IList<string> Errors { get; set; } = [];
    public IList<string> Warnings { get; set; } = [];
    public IList<string> Exceptions { get; set; } = [];
    [JsonIgnore]
    public IList<Exception> ExceptionObjects { get; set; } = [];
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public TimeSpan? Elapsed { get; set; }
    public int? ElapsedMilliseconds => (int?)Elapsed?.TotalMilliseconds;
    public required string WhoAmI { get; set; }
    public object? Data { get; set; }
}