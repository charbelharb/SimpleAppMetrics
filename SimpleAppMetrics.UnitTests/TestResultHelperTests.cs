using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using SimpleAppMetrics.UnitTests.MockTests;

namespace SimpleAppMetrics.UnitTests;

public class TestResultHelperTests
{
    [Fact]
    public void Constructor_WithNullTimeProvider_UseSystemTimeProvider()
    {
        // Act
        var helper = new TestResultHelper();

        // Assert
        Assert.NotNull(helper);
    }

    [Fact]
    public void Constructor_WithCustomTimeProvider_UsesProvidedTimeProvider()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();

        // Act
        var helper = new TestResultHelper(fakeTimeProvider);

        // Assert
        Assert.NotNull(helper);
    }

    [Fact]
    public void CreateFor_Generic_ReturnsResultWithTypeNameAsWhoAmI()
    {
        // Act
        var result = TestResultHelper.CreateFor<FooBarTest>();

        // Assert
        Assert.Equal("FooBarTest", result.WhoAmI);
        Assert.Equal(TestResultStatus.Pass, result.Status);
    }

    [Fact]
    public void CreateFor_WithTestInstance_ReturnsResultWithTypeNameAsWhoAmI()
    {
        // Arrange
        var mockTest = Substitute.For<ITest>();

        // Act
        var result = TestResultHelper.CreateFor(mockTest);

        // Assert
        Assert.Contains("ObjectProxy", result.WhoAmI);
    }

    [Fact]
    public void RunWithTiming_SetsStartDateEndDateAndElapsed()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.SetUtcNow(new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero));
        
        var helper = new TestResultHelper(fakeTimeProvider);
        var mockTest = Substitute.For<ITest>();
        var testResult = new DefaultTestResult { WhoAmI = "Test" };
        
        mockTest.Run().Returns(testResult);

        // Act
        fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(100));
        var result = helper.RunWithTiming(mockTest);

        // Assert
        Assert.NotNull(result.StartDate);
        Assert.NotNull(result.EndDate);
        Assert.NotNull(result.Elapsed);
        Assert.Equal(new DateTime(2024, 1, 1, 12, 0, 0,100, DateTimeKind.Utc), result.StartDate);
    }

    [Fact]
    public void RunWithTiming_WhenStartDateAlreadySet_DoesNotOverwrite()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var helper = new TestResultHelper(fakeTimeProvider);
        var mockTest = Substitute.For<ITest>();
        
        var existingStartDate = new DateTime(2023, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var testResult = new DefaultTestResult 
        { 
            WhoAmI = "Test",
            StartDate = existingStartDate
        };
        
        mockTest.Run().Returns(testResult);

        // Act
        var result = helper.RunWithTiming(mockTest);

        // Assert
        Assert.Equal(existingStartDate, result.StartDate);
    }

    [Fact]
    public void RunWithTiming_WhenEndDateAlreadySet_DoesNotOverwrite()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var helper = new TestResultHelper(fakeTimeProvider);
        var mockTest = Substitute.For<ITest>();
        
        var existingEndDate = new DateTime(2023, 1, 1, 11, 0, 0, DateTimeKind.Utc);
        var testResult = new DefaultTestResult 
        { 
            WhoAmI = "Test",
            EndDate = existingEndDate
        };
        
        mockTest.Run().Returns(testResult);

        // Act
        var result = helper.RunWithTiming(mockTest);

        // Assert
        Assert.Equal(existingEndDate, result.EndDate);
    }

    [Fact]
    public void RunWithTiming_WhenElapsedAlreadySet_DoesNotOverwrite()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var helper = new TestResultHelper(fakeTimeProvider);
        var mockTest = Substitute.For<ITest>();
        
        var existingElapsed = TimeSpan.FromSeconds(5);
        var testResult = new DefaultTestResult 
        { 
            WhoAmI = "Test",
            Elapsed = existingElapsed
        };
        
        mockTest.Run().Returns(testResult);

        // Act
        var result = helper.RunWithTiming(mockTest);

        // Assert
        Assert.Equal(existingElapsed, result.Elapsed);
    }

    [Fact]
    public async Task RunWithTimingAsync_SetsStartDateEndDateAndElapsed()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.SetUtcNow(new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero));
        
        var helper = new TestResultHelper(fakeTimeProvider);
        var mockTest = Substitute.For<ITest>();
        var testResult = new DefaultTestResult { WhoAmI = "Test" };
        
        mockTest.RunAsync(Arg.Any<CancellationToken>()).Returns(testResult);

        // Act
        fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(150));
        var result = await helper.RunWithTimingAsync(mockTest);

        // Assert
        Assert.NotNull(result.StartDate);
        Assert.NotNull(result.EndDate);
        Assert.NotNull(result.Elapsed);
        Assert.Equal(new DateTime(2024, 1, 1, 12, 0, 0,150, DateTimeKind.Utc), result.StartDate);
    }

    [Fact]
    public async Task RunWithTimingAsync_PassesCancellationToken()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var helper = new TestResultHelper(fakeTimeProvider);
        var mockTest = Substitute.For<ITest>();
        var testResult = new DefaultTestResult { WhoAmI = "Test" };
        var cts = new CancellationTokenSource();
        
        mockTest.RunAsync(Arg.Any<CancellationToken>()).Returns(testResult);

        // Act
        await helper.RunWithTimingAsync(mockTest, cts.Token);

        // Assert
        await mockTest.Received(1).RunAsync(cts.Token);
    }

    [Fact]
    public async Task RunWithTimingAsync_WhenStartDateAlreadySet_DoesNotOverwrite()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var helper = new TestResultHelper(fakeTimeProvider);
        var mockTest = Substitute.For<ITest>();
        
        var existingStartDate = new DateTime(2023, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var testResult = new DefaultTestResult 
        { 
            WhoAmI = "Test",
            StartDate = existingStartDate
        };
        
        mockTest.RunAsync(Arg.Any<CancellationToken>()).Returns(testResult);

        // Act
        var result = await helper.RunWithTimingAsync(mockTest);

        // Assert
        Assert.Equal(existingStartDate, result.StartDate);
    }

    [Fact]
    public void MeasureExecution_ReturnsResultAndElapsedTime()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var helper = new TestResultHelper(fakeTimeProvider);
        var mockTest = Substitute.For<ITest>();
        var testResult = new DefaultTestResult { WhoAmI = "Test" };
        
        mockTest.Run().Returns(_ =>
        {
            fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(200));
            return testResult;
        });

        // Act
        var (result, elapsed) = helper.MeasureExecution(mockTest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.WhoAmI);
        Assert.Equal(TimeSpan.FromMilliseconds(200), elapsed);
        Assert.Equal(TimeSpan.FromMilliseconds(200), result.Elapsed);
    }

    [Fact]
    public void MeasureExecution_WhenElapsedAlreadySet_DoesNotOverwrite()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var helper = new TestResultHelper(fakeTimeProvider);
        var mockTest = Substitute.For<ITest>();
        
        var existingElapsed = TimeSpan.FromSeconds(10);
        var testResult = new DefaultTestResult 
        { 
            WhoAmI = "Test",
            Elapsed = existingElapsed
        };
        
        mockTest.Run().Returns(_ =>
        {
            fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(200));
            return testResult;
        });

        // Act
        var (result, elapsed) = helper.MeasureExecution(mockTest);

        // Assert
        Assert.Equal(existingElapsed, result.Elapsed);
        Assert.Equal(TimeSpan.FromMilliseconds(200), elapsed);
    }

    [Fact]
    public async Task MeasureExecutionAsync_ReturnsResultAndElapsedTime()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var helper = new TestResultHelper(fakeTimeProvider);
        var mockTest = Substitute.For<ITest>();
        var testResult = new DefaultTestResult { WhoAmI = "Test" };
        
        mockTest.RunAsync(Arg.Any<CancellationToken>()).Returns(_ =>
        {
            fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(250));
            return Task.FromResult<ITestResult>(testResult);
        });

        // Act
        var (result, elapsed) = await helper.MeasureExecutionAsync(mockTest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.WhoAmI);
        Assert.Equal(TimeSpan.FromMilliseconds(250), elapsed);
        Assert.Equal(TimeSpan.FromMilliseconds(250), result.Elapsed);
    }

    [Fact]
    public async Task MeasureExecutionAsync_PassesCancellationToken()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var helper = new TestResultHelper(fakeTimeProvider);
        var mockTest = Substitute.For<ITest>();
        var testResult = new DefaultTestResult { WhoAmI = "Test" };
        var cts = new CancellationTokenSource();
        
        mockTest.RunAsync(Arg.Any<CancellationToken>()).Returns(testResult);

        // Act
        await helper.MeasureExecutionAsync(mockTest, cts.Token);

        // Assert
        await mockTest.Received(1).RunAsync(cts.Token);
    }

    [Fact]
    public async Task MeasureExecutionAsync_WhenElapsedAlreadySet_DoesNotOverwrite()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var helper = new TestResultHelper(fakeTimeProvider);
        var mockTest = Substitute.For<ITest>();
        
        var existingElapsed = TimeSpan.FromSeconds(15);
        var testResult = new DefaultTestResult 
        { 
            WhoAmI = "Test",
            Elapsed = existingElapsed
        };
        
        mockTest.RunAsync(Arg.Any<CancellationToken>()).Returns(_ =>
        {
            fakeTimeProvider.Advance(TimeSpan.FromMilliseconds(300));
            return Task.FromResult<ITestResult>(testResult);
        });

        // Act
        var (result, elapsed) = await helper.MeasureExecutionAsync(mockTest);

        // Assert
        Assert.Equal(existingElapsed, result.Elapsed);
        Assert.Equal(TimeSpan.FromMilliseconds(300), elapsed);
    }

    [Fact]
    public void MeasureExecution_WithRealTest_MeasuresAccurately()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var helper = new TestResultHelper(fakeTimeProvider);
        var realTest = new SlowTest(fakeTimeProvider, TimeSpan.FromMilliseconds(100));

        // Act
        var (result, elapsed) = helper.MeasureExecution(realTest);

        // Assert
        Assert.Equal(TestResultStatus.Pass, result.Status);
        Assert.Equal(TimeSpan.FromMilliseconds(100), elapsed);
    }

    [Fact]
    public async Task MeasureExecutionAsync_WithRealTest_MeasuresAccurately()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var helper = new TestResultHelper(fakeTimeProvider);
        var realTest = new SlowTest(fakeTimeProvider, TimeSpan.FromMilliseconds(125));

        // Act
        var (result, elapsed) = await helper.MeasureExecutionAsync(realTest);

        // Assert
        Assert.Equal(TestResultStatus.Pass, result.Status);
        Assert.Equal(TimeSpan.FromMilliseconds(125), elapsed);
    }

    [Fact]
    public void RunWithTiming_CallsRunOnTest()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var helper = new TestResultHelper(fakeTimeProvider);
        var mockTest = Substitute.For<ITest>();
        var testResult = new DefaultTestResult { WhoAmI = "Test" };
        
        mockTest.Run().Returns(testResult);

        // Act
        helper.RunWithTiming(mockTest);

        // Assert
        mockTest.Received(1).Run();
    }

    [Fact]
    public async Task RunWithTimingAsync_CallsRunAsyncOnTest()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var helper = new TestResultHelper(fakeTimeProvider);
        var mockTest = Substitute.For<ITest>();
        var testResult = new DefaultTestResult { WhoAmI = "Test" };
        
        mockTest.RunAsync(Arg.Any<CancellationToken>()).Returns(testResult);

        // Act
        await helper.RunWithTimingAsync(mockTest);

        // Assert
        await mockTest.Received(1).RunAsync(Arg.Any<CancellationToken>());
    }

    // Helper test class for integration tests
    private class SlowTest(FakeTimeProvider timeProvider, TimeSpan delay) : ITest
    {
        public ITestResult Run()
        {
            timeProvider.Advance(delay);
            return new DefaultTestResult 
            { 
                WhoAmI = "SlowTest",
                Status = TestResultStatus.Pass 
            };
        }

        public Task<ITestResult> RunAsync(CancellationToken cancellationToken = default)
        {
            timeProvider.Advance(delay);
            return Task.FromResult<ITestResult>(new DefaultTestResult 
            { 
                WhoAmI = "SlowTest",
                Status = TestResultStatus.Pass 
            });
        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}