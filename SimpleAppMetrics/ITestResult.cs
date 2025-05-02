using System.Text.Json.Serialization;

namespace SimpleAppMetrics;

public interface ITestResult
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TestResultStatus Status { get; set; }
    
    public TestResultStatus StatusCode { get; }

    public IList<string> SuccessMessages { get; set; }
    
    public IList<string> DegradedMessages { get; set; }
    
    public IList<string> Errors { get; set; }

    public IList<string> Warnings { get; set; }
    
    public IList<string> Exceptions { get; set; }
    
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public TimeSpan? Elapsed { get; set; }
    
    public int? ElapsedMilliseconds { get; }

    public string WhoAmI { get; set; }
    
    public object? Data { get; set; }
}