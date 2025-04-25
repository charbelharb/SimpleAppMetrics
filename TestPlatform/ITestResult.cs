namespace TestPlatform;

public interface ITestResult
{
    public TestResultStatus Status { get; set; }

    public IList<string> SuccessMessages { get; set; }
    
    public IList<string> DegradedMessages { get; set; }
    
    public IList<string> Errors { get; set; }

    public IList<string> Warnings { get; set; }
    
    public IList<Exception> Exceptions { get; set; }
    
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }

    public string WhoAmI { get; set; }
    
    public object? Data { get; set; }
}