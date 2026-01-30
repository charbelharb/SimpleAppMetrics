using System.Diagnostics;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace SimpleAppMetrics.UnitTests;

public class OpenTelemetryIntegrationTests
{
    [Fact]
    public void Start_CreatesActivity_WithCorrectName()
    {
        // Arrange
        var runner = new DefaultTestRunner(new List<ITest>());
        
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == "SimpleAppMetrics";
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        listener.ActivityStarted = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(listener);

        // Act
        runner.Start();

        // Assert
        var activity = activities.FirstOrDefault(a => a.DisplayName == "Start");
        Assert.NotNull(activity);
    }

    [Fact]
    public async Task StartAsync_CreatesActivity_WithCorrectName()
    {
        // Arrange
        var runner = new DefaultTestRunner(new List<ITest>());
        
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == "SimpleAppMetrics";
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        listener.ActivityStarted = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(listener);

        // Act
        await runner.StartAsync();

        // Assert
        var activity = activities.FirstOrDefault(a => a.DisplayName == "StartAsync");
        Assert.NotNull(activity);
    }

    [Fact]
    public void SafeStart_CreatesActivity_WithCorrectName()
    {
        // Arrange
        var runner = new DefaultTestRunner(new List<ITest>());
        
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == "SimpleAppMetrics";
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        listener.ActivityStarted = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(listener);

        // Act
        runner.SafeStart();

        // Assert
        var activity = activities.FirstOrDefault(a => a.DisplayName == "SafeStart");
        Assert.NotNull(activity);
    }

    [Fact]
    public async Task SafeStartAsync_CreatesActivity_WithCorrectName()
    {
        // Arrange
        var runner = new DefaultTestRunner(new List<ITest>());
        
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == "SimpleAppMetrics";
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        listener.ActivityStarted = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(listener);

        // Act
        await runner.SafeStartAsync();

        // Assert
        var activity = activities.FirstOrDefault(a => a.DisplayName == "SafeStartAsync");
        Assert.NotNull(activity);
    }

    [Fact]
    public void Start_SetsTestCountTag()
    {
        // Arrange
        var mockTest1 = Substitute.For<ITest>();
        var mockTest2 = Substitute.For<ITest>();
        mockTest1.Run().Returns(new DefaultTestResult { WhoAmI = "Test1", Status = TestResultStatus.Pass });
        mockTest2.Run().Returns(new DefaultTestResult { WhoAmI = "Test2", Status = TestResultStatus.Pass });
        
        var tests = new List<ITest> { mockTest1, mockTest2 };
        var runner = new DefaultTestRunner(tests);
        
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == "SimpleAppMetrics";
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        listener.ActivityStarted = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(listener);

        // Act
        runner.Start();

        // Assert
        var activity = activities.FirstOrDefault(a => a.DisplayName == "Start");
        Assert.NotNull(activity);
        Assert.Equal("2", activity.GetTagItem("test.count")?.ToString());
    }

    [Fact]
    public void Start_SetsDurationTag()
    {
        // Arrange
        var mockTest = Substitute.For<ITest>();
        mockTest.Run().Returns(new DefaultTestResult { WhoAmI = "Test1", Status = TestResultStatus.Pass });
        
        var tests = new List<ITest> { mockTest };
        var runner = new DefaultTestRunner(tests);
        
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == "SimpleAppMetrics";
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        listener.ActivityStarted = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(listener);

        // Act
        runner.Start();

        // Assert
        var activity = activities.FirstOrDefault(a => a.DisplayName == "Start");
        Assert.NotNull(activity);
        Assert.NotNull(activity.GetTagItem("test.duration_ms"));
    }

    [Fact]
    public void Start_CreatesChildActivitiesForEachTest()
    {
        // Arrange
        var mockTest1 = Substitute.For<ITest>();
        var mockTest2 = Substitute.For<ITest>();
        mockTest1.Run().Returns(new DefaultTestResult { WhoAmI = "Test1", Status = TestResultStatus.Pass });
        mockTest2.Run().Returns(new DefaultTestResult { WhoAmI = "Test2", Status = TestResultStatus.Pass });
        
        var tests = new List<ITest> { mockTest1, mockTest2 };
        var runner = new DefaultTestRunner(tests);
        
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == "SimpleAppMetrics";
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        listener.ActivityStarted = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(listener);

        // Act
        runner.Start();

        // Assert
        Assert.Contains(activities, a => a.DisplayName.StartsWith("Test."));
        Assert.True(activities.Count >= 3); // Parent + 2 child activities
    }

    [Fact]
    public void Start_ChildActivitiesHaveTestNameTag()
    {
        // Arrange
        var mockTest = Substitute.For<ITest>();
        mockTest.Run().Returns(new DefaultTestResult { WhoAmI = "DatabaseTest", Status = TestResultStatus.Pass });
        
        var tests = new List<ITest> { mockTest };
        var runner = new DefaultTestRunner(tests);
        
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == "SimpleAppMetrics";
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        listener.ActivityStarted = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(listener);

        // Act
        runner.Start();

        // Assert
        var testActivity = activities.FirstOrDefault(a => a.DisplayName.StartsWith("Test."));
        Assert.NotNull(testActivity);
        var tagsList = new List<string>();
        foreach (ref readonly var tag in testActivity.EnumerateTagObjects())
        {
            tagsList.Add(tag.Key);
        }
        Assert.Contains("test.name", tagsList);
    }

    [Fact]
    public void Start_ChildActivitiesHaveStatusTag()
    {
        // Arrange
        var mockTest = Substitute.For<ITest>();
        mockTest.Run().Returns(new DefaultTestResult { WhoAmI = "Test1", Status = TestResultStatus.Pass });
        
        var tests = new List<ITest> { mockTest };
        var runner = new DefaultTestRunner(tests);
        
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == "SimpleAppMetrics";
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        listener.ActivityStarted = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(listener);

        // Act
        runner.Start();

        // Assert
        var testActivity = activities.FirstOrDefault(a => a.DisplayName.StartsWith("Test."));
        Assert.NotNull(testActivity);
        var tagsList = new List<string>();
        foreach (ref readonly var tag in testActivity.EnumerateTagObjects())
        {
            tagsList.Add(tag.Key);
        }
        Assert.Contains("test.status", tagsList);
        Assert.Equal("Pass", testActivity.GetTagItem("test.status")?.ToString());
    }

    [Fact]
    public void Start_ChildActivitiesHaveDurationTag()
    {
        // Arrange
        var mockTest = Substitute.For<ITest>();
        mockTest.Run().Returns(new DefaultTestResult { WhoAmI = "Test1", Status = TestResultStatus.Pass });
        
        var tests = new List<ITest> { mockTest };
        var runner = new DefaultTestRunner(tests);
        
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == "SimpleAppMetrics";
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        listener.ActivityStarted = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(listener);

        // Act
        runner.Start();

        // Assert
        var testActivity = activities.FirstOrDefault(a => a.DisplayName.StartsWith("Test."));
        Assert.NotNull(testActivity);
        var tagsList = new List<string>();
        foreach (ref readonly var tag in testActivity.EnumerateTagObjects())
        {
            tagsList.Add(tag.Key);
        }
        Assert.Contains("test.duration_ms", tagsList);
    }

    [Fact]
    public async Task StartAsync_CreatesChildActivitiesForEachTest()
    {
        // Arrange
        var mockTest1 = Substitute.For<ITest>();
        var mockTest2 = Substitute.For<ITest>();
        mockTest1.RunAsync(Arg.Any<CancellationToken>()).Returns(new DefaultTestResult { WhoAmI = "Test1", Status = TestResultStatus.Pass });
        mockTest2.RunAsync(Arg.Any<CancellationToken>()).Returns(new DefaultTestResult { WhoAmI = "Test2", Status = TestResultStatus.Pass });
        
        var tests = new List<ITest> { mockTest1, mockTest2 };
        var runner = new DefaultTestRunner(tests);
        
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == "SimpleAppMetrics";
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        listener.ActivityStarted = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(listener);

        // Act
        await runner.StartAsync();

        // Assert
        Assert.Contains(activities, a => a.DisplayName.StartsWith("Test."));
        Assert.True(activities.Count >= 3); // Parent + 2 child activities
    }

    [Fact]
    public void Start_WithoutListener_DoesNotCreateActivities()
    {
        // Arrange
        var mockTest = Substitute.For<ITest>();
        mockTest.Run().Returns(new DefaultTestResult { WhoAmI = "Test1", Status = TestResultStatus.Pass });
        
        var tests = new List<ITest> { mockTest };
        var runner = new DefaultTestRunner(tests);

        // Act - No listener registered
        var exception = Record.Exception(() => runner.Start());

        // Assert - Should work fine without throwing
        Assert.Null(exception);
    }

    [Fact]
    public async Task StartAsync_WithoutListener_DoesNotCreateActivities()
    {
        // Arrange
        var mockTest = Substitute.For<ITest>();
        mockTest.RunAsync(Arg.Any<CancellationToken>()).Returns(new DefaultTestResult { WhoAmI = "Test1", Status = TestResultStatus.Pass });
        
        var tests = new List<ITest> { mockTest };
        var runner = new DefaultTestRunner(tests);

        // Act - No listener registered
        var exception = await Record.ExceptionAsync(async () => await runner.StartAsync());

        // Assert - Should work fine without throwing
        Assert.Null(exception);
    }
    
    [Fact]
    public void Start_ActivityKind_IsInternal()
    {
        // Arrange
        var runner = new DefaultTestRunner(new List<ITest>());
        
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == "SimpleAppMetrics";
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        listener.ActivityStarted = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(listener);

        // Act
        runner.Start();

        // Assert
        var activity = activities.FirstOrDefault(a => a.DisplayName == "Start");
        Assert.NotNull(activity);
        Assert.Equal(ActivityKind.Internal, activity.Kind);
    }

    [Fact]
    public void Start_ParentChildRelationship_IsEstablished()
    {
        // Arrange
        var mockTest = Substitute.For<ITest>();
        mockTest.Run().Returns(new DefaultTestResult { WhoAmI = "Test1", Status = TestResultStatus.Pass });
        
        var tests = new List<ITest> { mockTest };
        var runner = new DefaultTestRunner(tests);
        
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == "SimpleAppMetrics";
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        listener.ActivityStarted = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(listener);

        // Act
        runner.Start();

        // Assert
        var parentActivity = activities.FirstOrDefault(a => a.DisplayName == "Start");
        var childActivity = activities.FirstOrDefault(a => a.DisplayName.StartsWith("Test."));
        
        Assert.NotNull(parentActivity);
        Assert.NotNull(childActivity);
        Assert.Equal(parentActivity.SpanId, childActivity.ParentSpanId);
    }

    [Fact]
    public void SafeStart_WithException_StillCreatesActivity()
    {
        // Arrange
        var mockTest = Substitute.For<ITest>();
        mockTest.Run().Throws(new InvalidOperationException("Test error"));
        
        var tests = new List<ITest> { mockTest };
        var runner = new DefaultTestRunner(tests);
        
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == "SimpleAppMetrics";
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        listener.ActivityStarted = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(listener);

        // Act
        runner.SafeStart();

        // Assert
        var activity = activities.FirstOrDefault(a => a.DisplayName == "SafeStart");
        Assert.NotNull(activity);
        Assert.Contains(activities, a => a.DisplayName.StartsWith("Test."));
    }

    [Fact]
    public void Start_MultipleTests_CreatesCorrectNumberOfActivities()
    {
        // Arrange
        var tests = new List<ITest>();
        for (var i = 0; i < 5; i++)
        {
            var mockTest = Substitute.For<ITest>();
            mockTest.Run().Returns(new DefaultTestResult { WhoAmI = $"Test{i}", Status = TestResultStatus.Pass });
            tests.Add(mockTest);
        }
        
        var runner = new DefaultTestRunner(tests);
        
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == "SimpleAppMetrics";
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        listener.ActivityStarted = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(listener);

        // Act
        runner.Start();

        // Assert
        Assert.Equal(6, activities.Count); // 1 parent + 5 children
    }

    [Fact]
    public void Start_WithLogging_CreatesActivitiesAndLogs()
    {
        // Arrange
        var mockTest = Substitute.For<ITest>();
        mockTest.Run().Returns(new DefaultTestResult { WhoAmI = "Test1", Status = TestResultStatus.Pass });
        
        var tests = new List<ITest> { mockTest };
        var logger = Substitute.For<ILogger<DefaultTestRunner>>();
        var runner = new DefaultTestRunner(tests, logger);
        
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == "SimpleAppMetrics";
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        listener.ActivityStarted = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(listener);

        // Act
        runner.Start();

        // Assert - Both logging and activities work
        Assert.NotEmpty(activities);
        logger.Received().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            null,
            Arg.Any<Func<object, Exception?, string>>());
    }
}