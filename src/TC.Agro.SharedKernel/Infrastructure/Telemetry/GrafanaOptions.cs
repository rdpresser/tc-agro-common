namespace TC.Agro.SharedKernel.Infrastructure.Telemetry
{
    /// <summary>
    /// Grafana/OpenTelemetry configuration options
    /// Section: Telemetry:Grafana
    /// </summary>
    public sealed class GrafanaOptions
    {
        /// <summary>
        /// OTLP (OpenTelemetry Protocol) exporter configuration
        /// </summary>
        public OtlpSettings Otlp { get; set; } = new();

        /// <summary>
        /// Grafana Agent configuration (for local/remote agent)
        /// </summary>
        public AgentSettings Agent { get; set; } = new();

        /// <summary>
        /// Resolves the OTLP endpoint URL based on Agent configuration
        /// If Agent is enabled, endpoint is built from Agent.Host and ports
        /// Otherwise, returns the configured Otlp.Endpoint
        /// </summary>
        public string ResolveEndpoint()
        {
            if (!Agent.Enabled)
                return Otlp.Endpoint;

            var protocol = (Otlp.Protocol ?? "grpc").ToLowerInvariant();
            var port = protocol == "grpc"
                ? Agent.OtlpGrpcPort
                : Agent.OtlpHttpPort;

            var scheme = Otlp.Insecure ? "http" : "https";
            return $"{scheme}://{Agent.Host}:{port}";
        }

        public sealed class OtlpSettings
        {
            /// <summary>
            /// OTLP endpoint URL (default: http://localhost:4317)
            /// </summary>
            public string Endpoint { get; set; } = "http://localhost:4317";

            /// <summary>
            /// OTLP protocol: "grpc" or "http/protobuf" (default: grpc)
            /// </summary>
            public string Protocol { get; set; } = "grpc";

            /// <summary>
            /// Authorization headers in format: "key1=value1,key2=value2"
            /// </summary>
            public string? Headers { get; set; }

            /// <summary>
            /// Timeout for OTLP exports in seconds (default: 10)
            /// </summary>
            public int TimeoutSeconds { get; set; } = 10;

            /// <summary>
            /// Use insecure connection without TLS (default: true for localhost)
            /// </summary>
            public bool Insecure { get; set; } = true;
        }

        public sealed class AgentSettings
        {
            /// <summary>
            /// Grafana Agent hostname (default: localhost)
            /// </summary>
            public string Host { get; set; } = "localhost";

            /// <summary>
            /// Grafana Agent OTLP gRPC port (default: 4317)
            /// </summary>
            public int OtlpGrpcPort { get; set; } = 4317;

            /// <summary>
            /// Grafana Agent OTLP HTTP port (default: 4318)
            /// </summary>
            public int OtlpHttpPort { get; set; } = 4318;

            /// <summary>
            /// Grafana Agent metrics port (default: 12345)
            /// </summary>
            public int MetricsPort { get; set; } = 12345;

            /// <summary>
            /// Enable Grafana Agent exporting (default: true)
            /// When false, telemetry is generated but not exported to OTLP
            /// </summary>
            public bool Enabled { get; set; } = true;
        }
    }
}
