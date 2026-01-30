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
                var grafanaOptions = GrafanaHelper.Build(configuration);
                var logsEndpoint = grafanaOptions.Otlp.LogsEndpoint;

                if (string.IsNullOrWhiteSpace(logsEndpoint))
                {
                    var scheme = grafanaOptions.Otlp.Insecure ? "http" : "https";

                    if (grafanaOptions.Agent.Enabled)
                    {
                        logsEndpoint = $"{scheme}://{grafanaOptions.Agent.Host}:{grafanaOptions.Agent.OtlpHttpPort}/v1/logs";
                    }
                    else if (!string.IsNullOrWhiteSpace(grafanaOptions.Otlp.Endpoint))
                    {
                        logsEndpoint = $"{grafanaOptions.Otlp.Endpoint.TrimEnd('/')}/v1/logs";
                    }
                }

                if (configuration is IConfigurationRoot root && !string.IsNullOrWhiteSpace(logsEndpoint))
                    root["Serilog:WriteTo:1:Args:endpoint"] = logsEndpoint;

                // Read default configuration (sinks, levels, overrides)
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

                // --- Essential Enrichers ---
                loggerConfiguration.Enrich.With(new UtcToLocalTimeEnricher(timeZone));
                loggerConfiguration.Enrich.WithSpan();           // Ensures trace_id/span_id when available
                loggerConfiguration.Enrich.FromLogContext();

                // OpenTelemetry semantic conventions / resource consistency
                loggerConfiguration.Enrich.WithProperty("service.name", serviceName);
                loggerConfiguration.Enrich.WithProperty("service.namespace", serviceNamespace);
                loggerConfiguration.Enrich.WithProperty("service.version", serviceVersion);
                loggerConfiguration.Enrich.WithProperty("deployment.environment", environment);
                loggerConfiguration.Enrich.WithProperty("host.provider", "localhost");
                loggerConfiguration.Enrich.WithProperty("host.platform", "k3d_kubernetes_service");
                loggerConfiguration.Enrich.WithProperty("service.instance.id", instanceId);

                // NOTE: Console sink (JSON stdout) is already configured in appsettings.json via ReadFrom.Configuration()
                // No need to add WriteTo.Console() here to avoid duplicate log entries
                // Grafana Agent / Loki collects from stdout as configured in appsettings.Development.json and appsettings.Production.json
            }, preserveStaticLogger: true, writeToProviders: true);
        }
    }
}
