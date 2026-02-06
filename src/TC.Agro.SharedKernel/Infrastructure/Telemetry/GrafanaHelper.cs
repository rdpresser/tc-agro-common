namespace TC.Agro.SharedKernel.Infrastructure.Telemetry
{
    /// <summary>
    /// Helper for loading and building Grafana/OpenTelemetry configuration
    /// Configuration Section: Telemetry:Grafana
    /// Environment Variable Override: Use standard .NET format TELEMETRY__GRAFANA__* for runtime overrides
    /// </summary>
    public sealed class GrafanaHelper
    {
        private const string GrafanaSectionName = "Telemetry:Grafana";

        public GrafanaOptions GrafanaSettings { get; }

        public GrafanaHelper(IConfiguration configuration)
        {
            GrafanaSettings = configuration.GetSection(GrafanaSectionName).Get<GrafanaOptions>()
                ?? new GrafanaOptions();
        }

        /// <summary>
        /// Static factory method for convenient one-line configuration
        /// </summary>
        public static GrafanaOptions Build(IConfiguration configuration) =>
            new GrafanaHelper(configuration).GrafanaSettings;
    }
}
