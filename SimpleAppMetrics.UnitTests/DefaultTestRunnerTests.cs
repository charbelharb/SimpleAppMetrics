using Microsoft.Extensions.DependencyInjection;
using SimpleAppMetrics.UnitTests.MockTests;

namespace SimpleAppMetrics.UnitTests;

public class DefaultTestRunnerTests
{
    [Fact]
    public void DefaultTestRunner_Start_ShouldRunAllTests()
    {
        // Arrange
        var defaultRunner = new DefaultTestRunner(new List<ITest>
        {
            new FooBarTest(),
            new TuringTest()
        });
        using (defaultRunner)
        {
            // Act
            var result = defaultRunner.Start();
        
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
    }
    
    [Fact]
    public async Task DefaultTestRunner_StartAsync_ShouldRunAllTests()
    {
        // Arrange
        var defaultRunner = new DefaultTestRunner(new List<ITest>
        {
            new FooBarTest(),
            new TuringTest()
        });

        using (defaultRunner)
        {
            // Act
            var result = await defaultRunner.StartAsync();
        
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count); 
        }
        
    }

    [Fact]
    public void DefaultTestRunner_Start_ShouldThrowExceptionWhenTestDoes()
    {
        // Arrange
        var defaultRunner = new DefaultTestRunner(new List<ITest>
        {
            new TheAlwaysThrowingExceptionTest()
        });

        using (defaultRunner)
        {
            // Assert
            Assert.Throws<Exception>(() => defaultRunner.Start());
        }
    }
    
    [Fact]
    public async Task DefaultTestRunner_StartAsync_ShouldThrowExceptionWhenTestDoes()
    {
        // Arrange
        var defaultRunner = new DefaultTestRunner(new List<ITest>
        {
            new TheAlwaysThrowingExceptionTest()
        });

        using (defaultRunner)
        {
            // Assert
            await Assert.ThrowsAsync<Exception>(() => defaultRunner.StartAsync());
        }
    }
    
    [Fact]
    public void DefaultTestRunner_SafeStart_ShouldRunAllTests()
    {
        // Arrange
        var defaultRunner = new DefaultTestRunner(new List<ITest>
        {
            new FooBarTest(),
            new TuringTest()
        });

        using (defaultRunner)
        {
            // Act
            var result = defaultRunner.SafeStart();
        
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
    }
    
    [Fact]
    public void DefaultTestRunner_SafeStart_ShouldCatchException()
    {
        // Arrange
        var defaultRunner = new DefaultTestRunner(new List<ITest>
        {
            new TheAlwaysThrowingExceptionTest(),
            new FooBarTest(),
            new TuringTest()
        });

        using (defaultRunner)
        {
            // Assert
            var exception = Record.Exception(() => defaultRunner.SafeStart());
            Assert.Null(exception);
            var result = defaultRunner.LastRunResults;
            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count);
            var failedTest = defaultRunner.LastRunResults.FirstOrDefault(test => test.WhoAmI == nameof(TheAlwaysThrowingExceptionTest));
            Assert.NotNull(failedTest);
            Assert.Equal("Not today", failedTest.Exceptions.FirstOrDefault()?.Message);
        }
    }
    
    [Fact]
    public async Task DefaultTestRunner_SafeStartAsync_ShouldRunAllTests()
    {
        // Arrange
        var defaultRunner = new DefaultTestRunner(new List<ITest>
        {
            new FooBarTest(),
            new TuringTest()
        });

        using (defaultRunner)
        {
            // Act
            var result = await defaultRunner.SafeStartAsync();
        
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
    }
    
    [Fact]
    public async Task DefaultTestRunner_SafeStartAsync_ShouldCatchException()
    {
        // Arrange
        var defaultRunner = new DefaultTestRunner(new List<ITest>
        {
            new TheAlwaysThrowingExceptionTest(),
            new FooBarTest(),
            new TuringTest()
        });

        using (defaultRunner)
        {
            // Assert
            var exception = await Record.ExceptionAsync(() =>  defaultRunner.SafeStartAsync());
            Assert.Null(exception);
            var result = defaultRunner.LastRunResults;
            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count);
            var failedTest = defaultRunner.LastRunResults.FirstOrDefault(test => test.WhoAmI == nameof(TheAlwaysThrowingExceptionTest));
            Assert.NotNull(failedTest);
            Assert.Equal("Not today", failedTest.Exceptions.FirstOrDefault()?.Message);
        }
    }
    
    [Fact]
    public void DefaultTestRunner_ServiceCollectionConstructor_ShouldAddOnlyRegisteredServices()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ITest, FooBarTest>();
        serviceCollection.AddTransient<ITest, TuringTest>();
        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        var defaultRunner = new DefaultTestRunner(serviceProvider);

        using (defaultRunner)
        {
            // Act
            var result = defaultRunner.Start();
        
            // Assert
            Assert.Equal(2, result.Count);
        }
    }

    [Fact]
    public void DefaultTestRunner_ShouldDispose()
    {
        // Arrange
        var defaultRunner = new DefaultTestRunner(new List<ITest>
        {
            new FooBarTest(),
            new TuringTest()
        });
        using (defaultRunner)
        {
            // Act
            var result = defaultRunner.Start();
        
            // Assert
            Assert.NotNull(result);
            Assert.False(defaultRunner.IsDisposed);
        }
        Assert.True(defaultRunner.IsDisposed);
    }
}