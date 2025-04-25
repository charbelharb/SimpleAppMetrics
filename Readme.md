# Simple App Metrics

## Usage
1. Create tests that inherit from `ITest`.
2. Implement `Run` and/or `RunAsync` methods.
3. Register tests that need to be run with the default DI: `builder.Services.AddTransient<ITest, FooBarTest>();`
4. Register the default test runner: `builder.Services.AddTestRunner();`
5. Inject `ITestRunner` into your health check class and call runner.RunAsync() or runner.Run() to run all tests.

## Note
- Non-registered tests will not be run, this mechanism can be used to specify needed tests per services.
- Exceptions handling **should** be done in the test itself, default runner will throw exceptions, **unless** `SafeStart` or `SafeStartAsync` are used.