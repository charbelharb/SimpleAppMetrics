# Simple App Metrics

A lightweight, flexible C# library for running health checks and collecting metrics in .NET applications. Designed to be simple, extensible, and integrate seamlessly with ASP.NET Core's health check system.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![NuGet](https://img.shields.io/nuget/v/SimpleAppMetrics.svg)](https://www.nuget.org/packages/SimpleAppMetrics/)

## Features

- ✅ Simple interface-based test design
- ✅ Flexible status flags supporting complex scenarios
- ✅ Built-in ASP.NET Core health check integration
- ✅ Automatic timing and result tracking
- ✅ Safe execution with exception handling
- ✅ Fluent configuration API
- ✅ Dependency injection ready
- ✅ Fully async/await compatible
- ✅ Automatic ILogger integration (v2.1.0+)
- ✅ Built-in OpenTelemetry distributed tracing support (v2.1.0+)

## Installation

```bash
dotnet add package SimpleAppMetrics
```

## Quick Start

### 1. Create a Test

```csharp
using SimpleAppMetrics;

public class DatabaseHealthCheck : ITest
{
    private readonly IDbConnection _connection;
    
    public DatabaseHealthCheck(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<ITestResult> RunAsync(CancellationToken cancellationToken = default)
    {
        var result = new DefaultTestResult { WhoAmI = "Database Health Check" };
        
        try
        {
            await _connection.OpenAsync(cancellationToken);
            result.Status = TestResultStatus.Pass;
            result.SuccessMessages.Add("Database connection successful");
        }
        catch (Exception ex)
        {
            result.Status = TestResultStatus.Fail;
            result.Exceptions.Add(ex.Message);
            result.ExceptionObjects.Add(ex);
        }
        
        return result;
    }

    public ITestResult Run() => RunAsync().GetAwaiter().GetResult();
    
    public bool IsDisposed { get; private set; }
    
    public void Dispose()
    {
        _connection?.Dispose();
        IsDisposed = true;
    }
}
```

### 2. Register Services

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register your tests
builder.Services.AddTransient<ITest, DatabaseHealthCheck>();
builder.Services.AddTransient<ITest, ApiHealthCheck>();
builder.Services.AddTransient<ITest, CacheHealthCheck>();

// Register test runner
builder.Services.AddDefaultTestRunner()
    .WithTestResultHelper()
    .Build();
```

### 3. Run Tests

```csharp
public class HealthController : ControllerBase
{
    private readonly ITestRunner _testRunner;
    
    public HealthController(ITestRunner testRunner)
    {
        _testRunner = testRunner;
    }
    
    [HttpGet("health")]
    public async Task<IActionResult> GetHealth()
    {
        var results = await _testRunner.SafeStartAsync();
        
        var allHealthy = results.All(r => r.IsSuccess());
        return allHealthy ? Ok(results) : StatusCode(503, results);
    }
}
```

## Configuration Options

### Basic Configuration

```csharp
// Simple registration
builder.Services.AddDefaultTestRunner();
```

### With Test Result Helper

```csharp
// Adds timing utilities and helper methods
builder.Services.AddDefaultTestRunner()
    .WithTestResultHelper()
    .Build();
```

### With Custom TimeProvider (for testing)

```csharp
var fakeTimeProvider = new FakeTimeProvider();

builder.Services.AddDefaultTestRunner()
    .WithTestResultHelper(fakeTimeProvider)
    .Build();
```

### Using Options Delegate

```csharp
builder.Services.AddDefaultTestRunner(options =>
{
    options.UseTestResultHelper = true;
    options.TimeProvider = TimeProvider.System;
});
```

### Standalone Helper Registration

```csharp
// Register TestResultHelper independently
builder.Services.AddTestResultHelper();

// Or with custom TimeProvider
builder.Services.AddTestResultHelper(customTimeProvider);
```

## Logging (v2.1.0+)

**Logging is automatically enabled if `ILogger` is registered in your application.** SimpleAppMetrics will detect and use it without any additional configuration.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register logging (standard ASP.NET Core)
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

// Register SimpleAppMetrics - logging will be automatically injected
builder.Services.AddDefaultTestRunner()
    .WithTestResultHelper()
    .Build();
```

### Logging Output

When logging is configured, you'll see structured logs like:

```
[12:00:00 INF] Starting test execution for 3 tests
[12:00:00 DBG] Executing test: DatabaseHealthCheck
[12:00:00 INF] Test DatabaseHealthCheck completed with status Pass in 150ms
[12:00:00 DBG] Executing test: RedisHealthCheck
[12:00:00 ERR] Test RedisHealthCheck completed with status Fail in 200ms
[12:00:00 ERR] Test RedisHealthCheck exception: System.TimeoutException: Connection timeout
[12:00:00 DBG] Executing test: ApiHealthCheck
[12:00:00 INF] Test ApiHealthCheck completed with status Pass in 100ms
[12:00:01 INF] Test run completed: 3 total, 2 passed, 1 failed, 0 fatal in 450ms
```

Log levels are automatically selected based on test status:
- **Critical**: Fatal errors
- **Error**: Failed tests, exceptions
- **Warning**: Degraded performance, warnings
- **Information**: Successful tests, summaries
- **Debug**: Test execution details

## OpenTelemetry (v2.1.0+)

**OpenTelemetry tracing is automatically enabled when you configure it in your application.** SimpleAppMetrics uses the built-in .NET `ActivitySource` API for distributed tracing - no external dependencies required.

### Setup OpenTelemetry

```csharp
// Install OpenTelemetry packages (in your application):
// dotnet add package OpenTelemetry.Exporter.Console
// dotnet add package OpenTelemetry.Extensions.Hosting

var builder = WebApplication.CreateBuilder(args);

// Configure OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("SimpleAppMetrics")  // Subscribe to SimpleAppMetrics traces
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter());

// Register SimpleAppMetrics - tracing will work automatically
builder.Services.AddDefaultTestRunner()
    .WithTestResultHelper()
    .Build();
```

### Trace Output

When configured, each test run creates distributed traces:

```
Trace: TestRunner.SafeStartAsync (450ms)
├── Test.DatabaseHealthCheck (150ms)
│   └── Tags: test.name=DatabaseHealthCheck, test.status=Pass, test.duration_ms=150
├── Test.RedisHealthCheck (200ms)
│   └── Tags: test.name=RedisHealthCheck, test.status=Fail, test.duration_ms=200
└── Test.ApiHealthCheck (100ms)
    └── Tags: test.name=ApiHealthCheck, test.status=Pass, test.duration_ms=100
```

**Activity Source Details:**
- Name: `"SimpleAppMetrics"`
- Version: `"2.1.0"`

### Supported APM Tools

View traces in:
- **Jaeger**: http://localhost:16686
- **Zipkin**: http://localhost:9411
- **Azure Application Insights**
- **AWS X-Ray**
- **Google Cloud Trace**
- **Datadog**
- **New Relic**
- **Elastic APM**

### Complete OpenTelemetry Example

```csharp
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Configure OpenTelemetry with OTLP exporter (Jaeger, Zipkin, etc.)
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService("MyHealthCheckService", serviceVersion: "1.0.0"))
    .WithTracing(tracing => tracing
        .AddSource("SimpleAppMetrics")        // SimpleAppMetrics traces
        .AddAspNetCoreInstrumentation()       // HTTP request traces
        .AddHttpClientInstrumentation()       // HTTP client traces
        .AddOtlpExporter(options =>           // Export to APM tool
        {
            options.Endpoint = new Uri("http://localhost:4317");
        }));

// Register tests and runner
builder.Services.AddTransient<ITest, DatabaseHealthCheck>();
builder.Services.AddDefaultTestRunner();
```

## Complete Configuration Example (Logging + OpenTelemetry)

```csharp
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

// Configure OpenTelemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService("MyHealthCheckService", serviceVersion: "1.0.0"))
    .WithTracing(tracing => tracing
        .AddSource("SimpleAppMetrics")
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter()
        .AddOtlpExporter());

// Register tests
builder.Services.AddTransient<ITest, DatabaseHealthCheck>();
builder.Services.AddTransient<ITest, RedisHealthCheck>();

// Register SimpleAppMetrics - logging and tracing work automatically!
builder.Services.AddDefaultTestRunner()
    .WithTestResultHelper()
    .Build();

var app = builder.Build();
app.Run();
```

## Health Check Integration

### Register Health Check

```csharp
builder.Services.AddDefaultTestRunner()
    .WithTestResultHelper()
    .Build();

builder.Services.AddHealthChecks()
    .AddCheck<TestRunnerHealthCheck>("app-metrics");
```

### Configure Health Check Endpoint

```csharp
var app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                data = e.Value.Data
            })
        });
        await context.Response.WriteAsync(result);
    }
});
```

## Using TestResultHelper

The `TestResultHelper` provides utilities for automatic timing and result creation:

```csharp
public class MyHealthCheck : ITest
{
    private readonly TestResultHelper _helper;
    private readonly IMyService _service;
    
    public MyHealthCheck(TestResultHelper helper, IMyService service)
    {
        _helper = helper;
        _service = service;
    }

    public async Task<ITestResult> RunAsync(CancellationToken cancellationToken = default)
    {
        // Option 1: Automatic timing wrapper
        return await _helper.RunWithTimingAsync(this, cancellationToken);
        
        // Option 2: Measure specific operations
        var (result, elapsed) = await _helper.MeasureExecutionAsync(this, cancellationToken);
        Console.WriteLine($"Test took {elapsed.TotalMilliseconds}ms");
        return result;
        
        // Option 3: Create result with auto-populated WhoAmI
        var result = TestResultHelper.CreateFor<MyHealthCheck>();
        // ... perform test logic
        return result;
    }
}
```

## Test Result Status Flags

The library supports rich status reporting using flags:

```csharp
// Simple statuses
TestResultStatus.Pass
TestResultStatus.Fail
TestResultStatus.Fatal

// Individual flags
TestResultStatus.Degraded
TestResultStatus.Warning
TestResultStatus.Errors
TestResultStatus.Exceptions

// Combined statuses
TestResultStatus.PassWithWarning
TestResultStatus.PassWithDegraded
TestResultStatus.FailWithErrors
TestResultStatus.PassWithAllIssues  // Pass but with all types of issues
TestResultStatus.FailWithAllIssues  // "Fire the whole department"
```

## Extension Methods

```csharp
using SimpleAppMetrics;

var result = await test.RunAsync();

// Check status
if (result.IsSuccess()) { /* ... */ }
if (result.IsFailure()) { /* ... */ }
if (result.IsFatal()) { /* ... */ }
if (result.HasWarnings()) { /* ... */ }
if (result.IsDegraded()) { /* ... */ }

// Get summaries
var summary = result.GetSummary();
// Output: "DatabaseTest: Pass (150ms)"

var detailed = result.GetDetailedSummary();
// Output: Multi-line detailed summary with all messages

// Get all issues
var issues = result.GetAllIssues();
// Returns: Combined warnings, errors, exceptions, degraded messages
```

## Advanced Usage

### Custom Test Result

```csharp
public class CustomTestResult : ITestResult
{
    public TestResultStatus Status { get; set; }
    public IList<string> SuccessMessages { get; set; } = new List<string>();
    public IList<string> DegradedMessages { get; set; } = new List<string>();
    public IList<string> Errors { get; set; } = new List<string>();
    public IList<string> Warnings { get; set; } = new List<string>();
    public IList<string> Exceptions { get; set; } = new List<string>();
    public IList<Exception> ExceptionObjects { get; set; } = new List<Exception>();
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public TimeSpan? Elapsed { get; set; }
    public int? ElapsedMilliseconds => (int?)Elapsed?.TotalMilliseconds;
    public required string WhoAmI { get; set; }
    public object? Data { get; set; }
    
    // Add custom properties
    public string Environment { get; set; } = "Production";
    public Dictionary<string, object> Metadata { get; set; } = new();
}
```

### Safe vs Unsafe Execution

```csharp
// Unsafe: Exceptions will propagate
var results = await testRunner.StartAsync();

// Safe: Exceptions are caught and returned as Fatal results
var results = await testRunner.SafeStartAsync();
```

### Manual Timing Control

```csharp
public async Task<ITestResult> RunAsync(CancellationToken cancellationToken = default)
{
    var result = new DefaultTestResult { WhoAmI = "Custom Timing Test" };
    
    // Manually control timing for specific operations
    var operationStart = DateTime.UtcNow;
    
    await PerformCriticalOperation();
    
    result.StartDate = operationStart;
    result.EndDate = DateTime.UtcNow;
    result.Elapsed = result.EndDate - result.StartDate;
    
    result.Status = TestResultStatus.Pass;
    return result;
}
```

## Example: Complete Application

```csharp
using SimpleAppMetrics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Configure OpenTelemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService("HealthCheckService"))
    .WithTracing(tracing => tracing
        .AddSource("SimpleAppMetrics")
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter());

// Register tests
builder.Services.AddTransient<ITest, DatabaseHealthCheck>();
builder.Services.AddTransient<ITest, RedisHealthCheck>();
builder.Services.AddTransient<ITest, ApiHealthCheck>();

// Configure SimpleAppMetrics
builder.Services.AddDefaultTestRunner()
    .WithTestResultHelper()
    .Build();

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck<TestRunnerHealthCheck>("app-metrics");

var app = builder.Build();

// Health check endpoint
app.MapHealthChecks("/health");

// Custom metrics endpoint
app.MapGet("/metrics", async (ITestRunner runner, ILogger<Program> logger) =>
{
    logger.LogInformation("Metrics endpoint called");
    
    var results = await runner.SafeStartAsync();
    return Results.Ok(new
    {
        timestamp = DateTime.UtcNow,
        results = results.Select(r => new
        {
            test = r.WhoAmI,
            status = r.Status.ToString(),
            elapsed = r.ElapsedMilliseconds,
            summary = r.GetSummary()
        })
    });
});

app.Run();
```

## Changelog

### v2.1.0 - Logging and OpenTelemetry Support

#### New Features
- **Added**: Automatic `ILogger<DefaultTestRunner>` integration for structured logging
- **Added**: Automatic OpenTelemetry distributed tracing support via `ActivitySource`
- **Added**: Automatic parent-child activity (span) relationships for distributed tracing
- **Added**: Rich activity tags: `test.name`, `test.status`, `test.duration_ms`, `test.count`

#### How It Works
- **Logging**: Automatically enabled if `ILogger` is registered in your application via dependency injection
- **OpenTelemetry**: Automatically creates traces using built-in .NET `ActivitySource` - configure OpenTelemetry in your app to collect them

#### Improvements
- Test execution creates distributed traces with hierarchical spans
- Different log levels based on test status (Critical, Error, Warning, Information)
- Structured logging with test names, statuses, and durations
- Exception details logged with full context
- Test run summaries with pass/fail/fatal counts
- Zero performance overhead when logging/tracing is not configured
- **No external OpenTelemetry dependencies** - uses built-in .NET `ActivitySource`

#### Configuration

**Logging** - automatically used if registered:
```csharp
// Configure logging in your application
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

// SimpleAppMetrics will automatically use it
builder.Services.AddDefaultTestRunner();
```

**OpenTelemetry** - automatically creates traces if you configure collection:
```csharp
// Configure OpenTelemetry in your application
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("SimpleAppMetrics")  // Subscribe to SimpleAppMetrics traces
        .AddConsoleExporter());

// SimpleAppMetrics will automatically create traces
builder.Services.AddDefaultTestRunner();
```

### v2.0.1 - Breaking Changes

#### Breaking Changes
- **Removed**: `StatusCode` property from `ITestResult` (was redundant with `Status`)
- **Added**: `ExceptionObjects` property to `ITestResult` for storing actual exception objects

#### Migration Guide

```csharp
// Before v2.0.1
var statusCode = result.StatusCode; // This is removed

// After v2.0.1
var status = result.Status; // Use Status directly

// New feature: Store exception objects
result.Exceptions.Add(ex.Message);      // String representation
result.ExceptionObjects.Add(ex);        // Actual exception object
```

#### New Features
- Added `TestResultHelper` for automatic timing and result creation
- Added `TestRunnerHealthCheck` for ASP.NET Core health check integration
- Added fluent DI configuration API
- Added `TestResultExtensions` with helper methods
- Added `TimeProvider` support for deterministic testing

#### Improvements
- Enhanced DI registration with builder pattern
- Improved exception handling with `ExceptionObjects`
- Better testability with `TimeProvider` abstraction

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Support

If you encounter any issues or have questions, please [open an issue](https://github.com/charbelharb/SimpleAppMetrics/issues) on GitHub.
