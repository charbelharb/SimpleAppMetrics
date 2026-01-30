using SimpleAppMetrics.Extensions;

namespace SimpleAppMetrics.UnitTests;

public class TestResultExtensionsTests
{
    [Fact]
    public void IsSuccess_WhenStatusIsPass_ReturnsTrue()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "Test",
            Status = TestResultStatus.Pass
        };

        // Act & Assert
        Assert.True(result.IsSuccess());
    }

    [Fact]
    public void IsSuccess_WhenStatusIsPassWithWarning_ReturnsTrue()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "Test",
            Status = TestResultStatus.PassWithWarning
        };

        // Act & Assert
        Assert.True(result.IsSuccess());
    }

    [Fact]
    public void IsSuccess_WhenStatusIsFail_ReturnsFalse()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "Test",
            Status = TestResultStatus.Fail
        };

        // Act & Assert
        Assert.False(result.IsSuccess());
    }

    [Fact]
    public void IsFailure_WhenStatusIsFail_ReturnsTrue()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "Test",
            Status = TestResultStatus.Fail
        };

        // Act & Assert
        Assert.True(result.IsFailure());
    }

    [Fact]
    public void IsFailure_WhenStatusIsFailWithWarning_ReturnsTrue()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "Test",
            Status = TestResultStatus.FailWithWarning
        };

        // Act & Assert
        Assert.True(result.IsFailure());
    }

    [Fact]
    public void IsFatal_WhenStatusIsFatal_ReturnsTrue()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "Test",
            Status = TestResultStatus.Fatal
        };

        // Act & Assert
        Assert.True(result.IsFatal());
    }

    [Fact]
    public void IsFatal_WhenStatusIsPass_ReturnsFalse()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "Test",
            Status = TestResultStatus.Pass
        };

        // Act & Assert
        Assert.False(result.IsFatal());
    }

    [Fact]
    public void HasWarnings_WhenStatusHasWarningFlag_ReturnsTrue()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "Test",
            Status = TestResultStatus.PassWithWarning
        };

        // Act & Assert
        Assert.True(result.HasWarnings());
    }

    [Fact]
    public void HasWarnings_WhenStatusHasNoWarningFlag_ReturnsFalse()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "Test",
            Status = TestResultStatus.Pass
        };

        // Act & Assert
        Assert.False(result.HasWarnings());
    }

    [Fact]
    public void IsDegraded_WhenStatusHasDegradedFlag_ReturnsTrue()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "Test",
            Status = TestResultStatus.PassWithDegraded
        };

        // Act & Assert
        Assert.True(result.IsDegraded());
    }

    [Fact]
    public void HasErrors_WhenStatusHasErrorsFlag_ReturnsTrue()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "Test",
            Status = TestResultStatus.PassWithErrors
        };

        // Act & Assert
        Assert.True(result.HasErrors());
    }

    [Fact]
    public void HasExceptions_WhenStatusHasExceptionsFlag_ReturnsTrue()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "Test",
            Status = TestResultStatus.PassWithExceptions
        };

        // Act & Assert
        Assert.True(result.HasExceptions());
    }

    [Fact]
    public void GetSummary_ReturnsFormattedString()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "DatabaseTest",
            Status = TestResultStatus.Pass,
            Elapsed = TimeSpan.FromMilliseconds(150)
        };

        // Act
        var summary = result.GetSummary();

        // Assert
        Assert.Equal("DatabaseTest: Pass (150ms)", summary);
    }

    [Fact]
    public void GetSummary_WhenElapsedIsNull_ReturnsZeroMs()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "Test",
            Status = TestResultStatus.Pass
        };

        // Act
        var summary = result.GetSummary();

        // Assert
        Assert.Contains("(0ms)", summary);
    }

    [Fact]
    public void GetDetailedSummary_IncludesAllMessages()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "ComplexTest",
            Status = TestResultStatus.PassWithWarning,
            Elapsed = TimeSpan.FromMilliseconds(200),
            SuccessMessages = new List<string> { "Connection successful" },
            Warnings = new List<string> { "Slow response time" },
            Errors = new List<string> { "Minor error occurred" }
        };

        // Act
        var summary = result.GetDetailedSummary();

        // Assert
        Assert.Contains("ComplexTest", summary);
        Assert.Contains("PassWithWarning", summary);
        Assert.Contains("200ms", summary);
        Assert.Contains("Connection successful", summary);
        Assert.Contains("Slow response time", summary);
        Assert.Contains("Minor error occurred", summary);
    }

    [Fact]
    public void IsHealthy_WhenStatusIsExactlyPass_ReturnsTrue()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "Test",
            Status = TestResultStatus.Pass
        };

        // Act & Assert
        Assert.True(result.IsHealthy());
    }

    [Fact]
    public void IsHealthy_WhenStatusIsPassWithWarning_ReturnsFalse()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "Test",
            Status = TestResultStatus.PassWithWarning
        };

        // Act & Assert
        Assert.False(result.IsHealthy());
    }

    [Fact]
    public void IsPassingWithIssues_WhenPassWithWarning_ReturnsTrue()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "Test",
            Status = TestResultStatus.PassWithWarning
        };

        // Act & Assert
        Assert.True(result.IsPassingWithIssues());
    }

    [Fact]
    public void IsPassingWithIssues_WhenExactlyPass_ReturnsFalse()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "Test",
            Status = TestResultStatus.Pass
        };

        // Act & Assert
        Assert.False(result.IsPassingWithIssues());
    }

    [Fact]
    public void IsPassingWithIssues_WhenFail_ReturnsFalse()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "Test",
            Status = TestResultStatus.Fail
        };

        // Act & Assert
        Assert.False(result.IsPassingWithIssues());
    }

    [Fact]
    public void GetAllIssues_CombinesAllIssueMessages()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "Test",
            Status = TestResultStatus.PassWithAllIssues,
            Warnings = new List<string> { "Warning1", "Warning2" },
            Errors = new List<string> { "Error1" },
            Exceptions = new List<string> { "Exception1" },
            DegradedMessages = new List<string> { "Degraded1" }
        };

        // Act
        var issues = result.GetAllIssues().ToList();

        // Assert
        Assert.Equal(5, issues.Count);
        Assert.Contains("Warning1", issues);
        Assert.Contains("Warning2", issues);
        Assert.Contains("Error1", issues);
        Assert.Contains("Exception1", issues);
        Assert.Contains("Degraded1", issues);
    }

    [Fact]
    public void GetAllIssues_WhenNoIssues_ReturnsEmpty()
    {
        // Arrange
        var result = new DefaultTestResult
        {
            WhoAmI = "Test",
            Status = TestResultStatus.Pass
        };

        // Act
        var issues = result.GetAllIssues().ToList();

        // Assert
        Assert.Empty(issues);
    }
}