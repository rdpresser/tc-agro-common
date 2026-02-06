using TC.Agro.SharedKernel.Infrastructure.Telemetry;

namespace TC.Agro.SharedKernel.Api.Extensions
{
    /// <summary>
    /// Extension methods for configuring Serilog logging.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class SerilogExtensions
    {
        /// <summary>
        /// Configures Serilog with custom enrichers and OpenTelemetry semantic conventions.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="serviceName">The service name for telemetry.</param>
        /// <param name="serviceNamespace">The service namespace for telemetry.</param>
        /// <param name="version">The service version.</param>
        /// <returns>The configured host builder.</returns>
        public static IHostBuilder UseCustomSerilog(
            this IHostBuilder hostBuilder,
            IConfiguration configuration,
            string serviceName,
            string serviceNamespace,
            string version = "1.0.0")
        {
            return hostBuilder.UseSerilog((hostContext, services, loggerConfiguration) =>
            {
                // Read default configuration (sinks, levels, overrides from appsettings)
                loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
                loggerConfiguration.ReadFrom.Services(services);

                // Useful values
                var environment = configuration["ASPNETCORE_ENVIRONMENT"]?.ToLower() ?? "development";
                var serviceVersion = version;
                var instanceId = Environment.MachineName;

                // Timezone (if configured)
                var timeZoneId = configuration["TimeZone"] ?? "UTC";
                TimeZoneInfo timeZone;
                try
                {
                    timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                }
                catch
                {
                    // Fallback to UTC if TimeZone is invalid
                    timeZone = TimeZoneInfo.Utc;
                }

                // --- CORE: Log Context (essential for manual properties) ---
                loggerConfiguration.Enrich.FromLogContext();

                // --- CORRELATION: Managed by CorrelationMiddleware + TelemetryMiddleware ---
                // CorrelationMiddleware pushes "CorrelationId" to LogContext
                // TelemetryMiddleware pushes "correlation_id" to LogContext
                // CorrelationIdNormalizationEnricher normalizes to "correlation_id" for Loki
                // NOTE: Serilog.Enrichers.CorrelationId (WithCorrelationId/WithCorrelationIdHeader)
                // was removed because it generates its own CorrelationId from HttpContext.TraceIdentifier,
                // overwriting the value set by CorrelationMiddleware via LogContext.
                loggerConfiguration.Enrich.With<CorrelationIdNormalizationEnricher>();

                // --- TRACES: Automatic trace_id/span_id from OpenTelemetry Activity ---
                // NOTE: Requires Serilog.Enrichers.Span NuGet package
                // Reads Activity.Current.TraceId and SpanId
                // This is the CRITICAL link between Logs in Loki ↔ Traces in Tempo
                loggerConfiguration.Enrich.WithSpan();

                // --- TIMEZONE: Local time conversion for human readability ---
                loggerConfiguration.Enrich.With(new UtcToLocalTimeEnricher(timeZone));

                // --- OTEL SEMANTIC CONVENTIONS: Resource attributes ---
                // NOTE: service.name is already in OTEL Resource, logs inherit automatically
                // Only add complementary metadata
                loggerConfiguration.Enrich.WithProperty("service.version", serviceVersion);
                loggerConfiguration.Enrich.WithProperty("service.instance.id", instanceId);
                loggerConfiguration.Enrich.WithProperty("deployment.environment", environment);
                loggerConfiguration.Enrich.WithProperty("host.name", Environment.MachineName);
                loggerConfiguration.Enrich.WithProperty("host.provider", "localhost");
                loggerConfiguration.Enrich.WithProperty("host.platform", "k3d_kubernetes_service");

                // NOTE: Console sink (JSON stdout) is already configured in appsettings.json via ReadFrom.Configuration()
                // Grafana Agent / Loki collects from stdout
                // OTLP exporter sends to OTEL Collector → Loki (v2.9+)
                // WithSpan() automatically adds trace_id/span_id when Activity.Current exists
            }, preserveStaticLogger: true, writeToProviders: true);
        }
    }
}
