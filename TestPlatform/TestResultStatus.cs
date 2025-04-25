namespace TestPlatform;


/// <summary>
/// Use any not pre-defined combination to fill in the gaps.
/// Go wild with the flags
/// </summary>
[Flags]
public enum TestResultStatus : byte
{
    /// <summary> No test was executed - skipped or status is unknown. </summary>
    None = 0,

    /// <summary> Use this when it's a 100% pass </summary>
    Pass = 1 << 0,

    /// <summary> Use this when it's a 100% fail </summary>
    Fail = 1 << 1,
    
    /// <summary> Use this when exceptions are thrown in SafeStart or SafeStartAsync </summary>
    Fatal = 1 << 2,

    /// <summary> Use this when performance is degraded but result is unknown </summary>
    Degraded = 1 << 3,

    /// <summary> Use this when something odd is detected but result is unknown </summary>
    Warning = 1 << 4,

    /// <summary> Use this when there are errors but result is unknown </summary>
    Errors = 1 << 5,

    /// <summary> Use this when exceptions are caught but result is unknown </summary>
    Exceptions = 1 << 6,

    /// <summary> Use this when performance is degraded but result is a pass </summary>
    PassWithDegraded = Pass | Degraded,

    /// <summary> Use this when something odd is detected but result is a pass </summary>
    PassWithWarning = Pass | Warning,

    /// <summary> Use this when there are errors but result is a pass </summary>
    PassWithErrors = Pass | Errors,

    /// <summary> Use this when exceptions are caught but result is a pass </summary>
    PassWithExceptions = Pass | Exceptions,

    /// <summary> Use this when performance is degraded and something odd is detected, but result is a pass </summary>
    PassWithDegradedAndWarning = Pass | Degraded | Warning,

    /// <summary> Use this when performance is degraded and there are errors, but result is a pass </summary>
    PassWithDegradedAndErrors = Pass | Degraded | Errors,

    /// <summary> Use this when performance is degraded and exceptions are caught, but result is a pass </summary>
    PassWithDegradedAndExceptions = Pass | Degraded | Exceptions,

    /// <summary> Use this when something odd is detected and there are errors, but result is a pass </summary>
    PassWithWarningAndErrors = Pass | Warning | Errors,
    
    /// <summary> Use this when something odd is detected and exceptions are caught, but result is a pass </summary>
    PassWithWarningAndExceptions = Pass | Warning | Exceptions,
    
    /// <summary> Use this when there are errors and exceptions are caught, but result is a pass </summary>
    PassWithErrorsAndExceptions = Pass | Errors | Exceptions,
    
    /// <summary> Use this when performance is degraded, something odd is detected and there are errors, but result is a pass </summary>
    PassWithDegradedWarningAndErrors = Pass | Degraded | Warning | Errors,
    
    /// <summary> Really, but sure.</summary>
    PassWithAllIssues = Pass | Degraded | Warning | Errors | Exceptions,
    
    /// <summary> Use this when performance is degraded but result is a fail </summary>
    FailWithDegraded = Fail | Degraded,

    /// <summary> Use this when something odd is detected but result is a fail </summary>
    FailWithWarning = Fail | Warning,

    /// <summary> Use this when there are errors but result is a fail </summary>
    FailWithErrors = Fail | Errors,

    /// <summary> Use this when exceptions are caught but result is a fail </summary>
    FailWithExceptions = Fail | Exceptions,

    /// <summary> Use this when performance is degraded and something odd is detected, but result is a fail </summary>
    FailWithDegradedAndWarning = Fail | Degraded | Warning,
    
    /// <summary> Use this when performance is degraded and there are errors, but result is a fail </summary>
    FailWithDegradedAndErrors = Fail | Degraded | Errors,
    
    /// <summary> Use this when performance is degraded and exceptions are caught, but result is a fail </summary>
    FailWithDegradedAndExceptions = Fail | Degraded | Exceptions,
    
    /// <summary> Use this when something odd is detected and there are errors, but result is a fail </summary>
    FailWithWarningAndErrors = Fail | Warning | Errors,
    
    /// <summary> Use this when something odd is detected and exceptions are caught, but result is a fail </summary>
    FailWithWarningAndExceptions = Fail | Warning | Exceptions,
    
    /// <summary> Use this when there are errors and exceptions are caught, but result is a fail </summary>
    FailWithErrorsAndExceptions = Fail | Errors | Exceptions,
    
    /// <summary> Use this when performance is degraded, something odd is detected and there are errors, but result is a fail </summary>
    FailWithDegradedWarningAndErrors = Fail | Degraded | Warning | Errors,
    
    /// <summary> Fire the whole department </summary>
    FailWithAllIssues = Fail | Degraded | Warning | Errors | Exceptions,
}