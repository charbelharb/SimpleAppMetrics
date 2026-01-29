using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace SimpleAppMetrics.UnitTests;

public class TestRunnerHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_WhenAllTestsPass_ReturnsHealthy()
    {
        // Arrange
        var mockRunner = Substitute.For<ITestRunner>();
        var results = new List<ITestResult>
        {
            new DefaultTestResult { WhoAmI = "Test1", Status = TestResultStatus.Pass },
            new DefaultTestResult { WhoAmI = "Test2", Status = TestResultStatus.Pass }
        };
        mockRunner.SafeStartAsync(Arg.Any<CancellationToken>()).Returns(results);

        var healthCheck = new TestRunnerHealthCheck(mockRunner);

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Equal("All tests passed", result.Description);
        Assert.Equal(2, result.Data["TotalTests"]);
        Assert.Equal(2, result.Data["PassedTests"]);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenTestFails_ReturnsUnhealthy()
    {
        // Arrange
        var mockRunner = Substitute.For<ITestRunner>();
        var results = new List<ITestResult>
        {
            new DefaultTestResult { WhoAmI = "Test1", Status = TestResultStatus.Pass },
            new DefaultTestResult { WhoAmI = "Test2", Status = TestResultStatus.Fail }
        };
        mockRunner.SafeStartAsync(Arg.Any<CancellationToken>()).Returns(results);

        var healthCheck = new TestRunnerHealthCheck(mockRunner);

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("Test failures detected", result.Description);
        Assert.Equal(1, result.Data["FailedTests"]);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenTestIsFatal_ReturnsUnhealthy()
    {
        // Arrange
        var mockRunner = Substitute.For<ITestRunner>();
        var results = new List<ITestResult>
        {
            new DefaultTestResult { WhoAmI = "Test1", Status = TestResultStatus.Fatal }
        };
        mockRunner.SafeStartAsync(Arg.Any<CancellationToken>()).Returns(results);

        var healthCheck = new TestRunnerHealthCheck(mockRunner);

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("Fatal test failures detected", result.Description);
        Assert.Equal(1, result.Data["FatalTests"]);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenTestIsDegraded_ReturnsDegraded()
    {
        // Arrange
        var mockRunner = Substitute.For<ITestRunner>();
        var results = new List<ITestResult>
        {
            new DefaultTestResult 
            { 
                WhoAmI = "Test1", 
                Status = TestResultStatus.PassWithDegraded 
            }
        };
        mockRunner.SafeStartAsync(Arg.Any<CancellationToken>()).Returns(results);

        var healthCheck = new TestRunnerHealthCheck(mockRunner);

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.Equal(HealthStatus.Degraded, result.Status);
        Assert.Equal("Performance degraded", result.Description);
        Assert.Equal(1, result.Data["DegradedTests"]);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenTestHasWarnings_ReturnsDegraded()
    {
        // Arrange
        var mockRunner = Substitute.For<ITestRunner>();
        var results = new List<ITestResult>
        {
            new DefaultTestResult 
            { 
                WhoAmI = "Test1", 
                Status = TestResultStatus.PassWithWarning 
            }
        };
        mockRunner.SafeStartAsync(Arg.Any<CancellationToken>()).Returns(results);

        var healthCheck = new TestRunnerHealthCheck(mockRunner);

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.Equal(HealthStatus.Degraded, result.Status);
        Assert.Equal("Warnings detected", result.Description);
        Assert.Equal(1, result.Data["TestsWithWarnings"]);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenExceptionThrown_ReturnsUnhealthy()
    {
        // Arrange
        var mockRunner = Substitute.For<ITestRunner>();
        var exception = new InvalidOperationException("Test exception");
        mockRunner.SafeStartAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(exception);

        var healthCheck = new TestRunnerHealthCheck(mockRunner);

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("Health check execution failed", result.Description);
        Assert.Equal(exception, result.Exception);
    }

    [Fact]
    public async Task CheckHealthAsync_IncludesProblemTestsInData()
    {
        // Arrange
        var mockRunner = Substitute.For<ITestRunner>();
        var results = new List<ITestResult>
        {
            new DefaultTestResult 
            { 
                WhoAmI = "Test1", 
                Status = TestResultStatus.Fail,
                Elapsed = TimeSpan.FromMilliseconds(100)
            }
        };
        mockRunner.SafeStartAsync(Arg.Any<CancellationToken>()).Returns(results);

        var healthCheck = new TestRunnerHealthCheck(mockRunner);

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.True(result.Data.ContainsKey("ProblemTests"));
        var problemTests = (string[])result.Data["ProblemTests"];
        Assert.Single(problemTests);
        Assert.Contains("Test1: Fail (100ms)", problemTests);
    }

    [Fact]
    public async Task CheckHealthAsync_FatalTakesPriorityOverFail()
    {
        // Arrange
        var mockRunner = Substitute.For<ITestRunner>();
        var results = new List<ITestResult>
        {
            new DefaultTestResult { WhoAmI = "Test1", Status = TestResultStatus.Fail },
            new DefaultTestResult { WhoAmI = "Test2", Status = TestResultStatus.Fatal }
        };
        mockRunner.SafeStartAsync(Arg.Any<CancellationToken>()).Returns(results);

        var healthCheck = new TestRunnerHealthCheck(mockRunner);

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("Fatal test failures detected", result.Description);
    }

    [Fact]
    public async Task CheckHealthAsync_PassesCancellationToken()
    {
        // Arrange
        var mockRunner = Substitute.For<ITestRunner>();
        var results = new List<ITestResult>
        {
            new DefaultTestResult { WhoAmI = "Test1", Status = TestResultStatus.Pass }
        };
        var cts = new CancellationTokenSource();
        
        mockRunner.SafeStartAsync(Arg.Any<CancellationToken>()).Returns(results);

        var healthCheck = new TestRunnerHealthCheck(mockRunner);

        // Act
        await healthCheck.CheckHealthAsync(new HealthCheckContext(), cts.Token);

        // Assert
        await mockRunner.Received(1).SafeStartAsync(cts.Token);
    }

    [Fact]
    public void Constructor_WhenTestRunnerIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TestRunnerHealthCheck(null!));
    }

    [Fact]
    public async Task CheckHealthAsync_WithMixedResults_ReturnsCorrectCounts()
    {
        // Arrange
        var mockRunner = Substitute.For<ITestRunner>();
        var results = new List<ITestResult>
        {
            new DefaultTestResult { WhoAmI = "Test1", Status = TestResultStatus.Pass },
            new DefaultTestResult { WhoAmI = "Test2", Status = TestResultStatus.PassWithWarning },
            new DefaultTestResult { WhoAmI = "Test3", Status = TestResultStatus.PassWithDegraded },
            new DefaultTestResult { WhoAmI = "Test4", Status = TestResultStatus.Fail },
            new DefaultTestResult { WhoAmI = "Test5", Status = TestResultStatus.Fatal }
        };
        mockRunner.SafeStartAsync(Arg.Any<CancellationToken>()).Returns(results);

        var healthCheck = new TestRunnerHealthCheck(mockRunner);

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.Equal(5, result.Data["TotalTests"]);
        Assert.Equal(3, result.Data["PassedTests"]); // Pass, PassWithWarning, PassWithDegraded
        Assert.Equal(1, result.Data["FailedTests"]);
        Assert.Equal(1, result.Data["FatalTests"]);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenNoTests_ReturnsHealthy()
    {
        // Arrange
        var mockRunner = Substitute.For<ITestRunner>();
        var results = new List<ITestResult>();
        mockRunner.SafeStartAsync(Arg.Any<CancellationToken>()).Returns(results);

        var healthCheck = new TestRunnerHealthCheck(mockRunner);

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Equal(0, result.Data["TotalTests"]);
    }

    [Fact]
    public async Task CheckHealthAsync_WithMultipleFatalTests_IncludesAllInProblemTests()
    {
        // Arrange
        var mockRunner = Substitute.For<ITestRunner>();
        var results = new List<ITestResult>
        {
            new DefaultTestResult 
            { 
                WhoAmI = "Test1", 
                Status = TestResultStatus.Fatal,
                Elapsed = TimeSpan.FromMilliseconds(50)
            },
            new DefaultTestResult 
            { 
                WhoAmI = "Test2", 
                Status = TestResultStatus.Fatal,
                Elapsed = TimeSpan.FromMilliseconds(75)
            }
        };
        mockRunner.SafeStartAsync(Arg.Any<CancellationToken>()).Returns(results);

        var healthCheck = new TestRunnerHealthCheck(mockRunner);

        // Act
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        var problemTests = (string[])result.Data["ProblemTests"];
        Assert.Equal(2, problemTests.Length);
        Assert.Contains("Test1: Fatal (50ms)", problemTests);
        Assert.Contains("Test2: Fatal (75ms)", problemTests);
    }
}