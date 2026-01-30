using OpenTelemetry.Trace;

namespace SimpleAppMetrics.Extensions;

/// <summary>
/// Extension methods for OpenTelemetry configuration
/// </summary>
public static class OpenTelemetryExtensions
{
    /// <summary>
    /// Adds SimpleAppMetrics instrumentation to OpenTelemetry tracing
    /// </summary>
    public static TracerProviderBuilder AddSimpleAppMetricsInstrumentation(this TracerProviderBuilder builder)
    {
        return builder.AddSource("SimpleAppMetrics");
    }
}