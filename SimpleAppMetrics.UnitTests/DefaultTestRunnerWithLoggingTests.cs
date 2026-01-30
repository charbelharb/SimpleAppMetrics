using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace SimpleAppMetrics.UnitTests;

public class DefaultTestRunnerWithLoggingTests
{
    [Fact]
    public void Constructor_WithNullLogger_DoesNotThrow()
    {
        // Act
        var runner = new DefaultTestRunner(new List<ITest>());

        // Assert
        Assert.NotNull(runner);
    }

    [Fact]
    public void Constructor_WithLogger_StoresLogger()
    {
        // Arrange
        var logger = Substitute.For<ILogger<DefaultTestRunner>>();

        // Act
        var runner = new DefaultTestRunner(new List<ITest>(), logger);

        // Assert
        Assert.NotNull(runner);
    }

    [Fact]
    public void Start_WithLogger_LogsTestExecution()
    {
        // Arrange
        var mockTest = Substitute.For<ITest>();
        var testResult = new DefaultTestResult { WhoAmI = "Test1", Status = TestResultStatus.Pass };
        mockTest.Run().Returns(testResult);

        var tests = new List<ITest> { mockTest };
        var logger = Substitute.For<ILogger<DefaultTestRunner>>();
        var runner = new DefaultTestRunner(tests, logger);

        // Act
        runner.Start();

        // Assert
        logger.Received().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Starting test execution")),
            null,
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task StartAsync_WithLogger_LogsTestExecution()
    {
        // Arrange
        var mockTest = Substitute.For<ITest>();
        var testResult = new DefaultTestResult { WhoAmI = "Test1", Status = TestResultStatus.Pass };
        mockTest.RunAsync(Arg.Any<CancellationToken>()).Returns(testResult);

        var tests = new List<ITest> { mockTest };
        var logger = Substitute.For<ILogger<DefaultTestRunner>>();
        var runner = new DefaultTestRunner(tests, logger);

        // Act
        await runner.StartAsync();

        // Assert
        logger.Received().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Starting async test execution")),
            null,
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public void SafeStart_WithLogger_LogsFatalErrors()
    {
        // Arrange
        var mockTest = Substitute.For<ITest>();
        var exception = new InvalidOperationException("Test error");
        mockTest.Run().Throws(exception);

        var tests = new List<ITest> { mockTest };
        var logger = Substitute.For<ILogger<DefaultTestRunner>>();
        var runner = new DefaultTestRunner(tests, logger);

        // Act
        runner.SafeStart();

        // Assert
        logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Fatal error")),
            exception,
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task SafeStartAsync_WithLogger_LogsFatalErrors()
    {
        // Arrange
        var mockTest = Substitute.For<ITest>();
        var exception = new InvalidOperationException("Test error");
        mockTest.RunAsync(Arg.Any<CancellationToken>()).Throws(exception);

        var tests = new List<ITest> { mockTest };
        var logger = Substitute.For<ILogger<DefaultTestRunner>>();
        var runner = new DefaultTestRunner(tests, logger);

        // Act
        await runner.SafeStartAsync();

        // Assert
        logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Fatal error")),
            exception,
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public void Start_LogsTestResultWithAppropriateLevel()
    {
        // Arrange
        var passTest = Substitute.For<ITest>();
        var passResult = new DefaultTestResult { WhoAmI = "PassTest", Status = TestResultStatus.Pass };
        passTest.Run().Returns(passResult);

        var failTest = Substitute.For<ITest>();
        var failResult = new DefaultTestResult { WhoAmI = "FailTest", Status = TestResultStatus.Fail };
        failTest.Run().Returns(failResult);

        var fatalTest = Substitute.For<ITest>();
        var fatalResult = new DefaultTestResult { WhoAmI = "FatalTest", Status = TestResultStatus.Fatal };
        fatalTest.Run().Returns(fatalResult);

        var tests = new List<ITest> { passTest, failTest, fatalTest };
        var logger = Substitute.For<ILogger<DefaultTestRunner>>();
        var runner = new DefaultTestRunner(tests, logger);

        // Act
        runner.Start();

        // Assert - Check for Information level (Pass)
        logger.Received().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("PassTest")),
            null,
            Arg.Any<Func<object, Exception?, string>>());

        // Assert - Check for Error level (Fail)
        logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("FailTest")),
            null,
            Arg.Any<Func<object, Exception?, string>>());

        // Assert - Check for Critical level (Fatal)
        logger.Received().Log(
            LogLevel.Critical,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("FatalTest")),
            null,
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public void Start_LogsTestRunSummary()
    {
        // Arrange
        var passTest = Substitute.For<ITest>();
        var passResult = new DefaultTestResult { WhoAmI = "PassTest", Status = TestResultStatus.Pass };
        passTest.Run().Returns(passResult);

        var failTest = Substitute.For<ITest>();
        var failResult = new DefaultTestResult { WhoAmI = "FailTest", Status = TestResultStatus.Fail };
        failTest.Run().Returns(failResult);

        var tests = new List<ITest> { passTest, failTest };
        var logger = Substitute.For<ILogger<DefaultTestRunner>>();
        var runner = new DefaultTestRunner(tests, logger);

        // Act
        runner.Start();

        // Assert
        logger.Received().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Test run completed")),
            null,
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public void Start_WithoutLogger_DoesNotThrow()
    {
        // Arrange
        var mockTest = Substitute.For<ITest>();
        var testResult = new DefaultTestResult { WhoAmI = "Test1", Status = TestResultStatus.Pass };
        mockTest.Run().Returns(testResult);

        var tests = new List<ITest> { mockTest };
        var runner = new DefaultTestRunner(tests);

        // Act & Assert
        var exception = Record.Exception(() => runner.Start());
        Assert.Null(exception);
    }

    [Fact]
    public void Dispose_WithLogger_LogsDisposal()
    {
        // Arrange
        var logger = Substitute.For<ILogger<DefaultTestRunner>>();
        var runner = new DefaultTestRunner(new List<ITest>(), logger);

        // Act
        runner.Dispose();

        // Assert
        logger.Received().Log(
            LogLevel.Debug,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Disposing test runner")),
            null,
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public void Start_WithDegradedTest_LogsWarning()
    {
        // Arrange
        var mockTest = Substitute.For<ITest>();
        var testResult = new DefaultTestResult 
        { 
            WhoAmI = "DegradedTest", 
            Status = TestResultStatus.PassWithDegraded 
        };
        mockTest.Run().Returns(testResult);

        var tests = new List<ITest> { mockTest };
        var logger = Substitute.For<ILogger<DefaultTestRunner>>();
        var runner = new DefaultTestRunner(tests, logger);

        // Act
        runner.Start();

        // Assert
        logger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("DegradedTest")),
            null,
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public void Start_WithTestExceptions_LogsExceptions()
    {
        // Arrange
        var mockTest = Substitute.For<ITest>();
        var testResult = new DefaultTestResult 
        { 
            WhoAmI = "TestWithException", 
            Status = TestResultStatus.PassWithExceptions,
            Exceptions = new List<string> { "Test exception message" }
        };
        mockTest.Run().Returns(testResult);

        var tests = new List<ITest> { mockTest };
        var logger = Substitute.For<ILogger<DefaultTestRunner>>();
        var runner = new DefaultTestRunner(tests, logger);

        // Act
        runner.Start();

        // Assert
        logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("exception")),
            null,
            Arg.Any<Func<object, Exception?, string>>());
    }
}