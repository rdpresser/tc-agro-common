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
        /// Resolves the base OTLP endpoint URL without /v1/* paths
        /// If Agent is enabled, endpoint is built from Agent.Host and ports
        /// Otherwise, returns the configured Otlp.Endpoint
        /// Internal method used by specific telemetry type resolvers
        /// </summary>
        private string ResolveBaseEndpoint()
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

        /// <summary>
        /// Resolves the OTLP endpoint URL for traces (/v1/traces)
        /// Per OTLP specification, traces are sent to /v1/traces endpoint
        /// </summary>
        public string ResolveTracesEndpoint()
        {
            var baseEndpoint = ResolveBaseEndpoint();
            if ((Otlp.Protocol ?? "grpc").Equals("grpc", StringComparison.OrdinalIgnoreCase))
                return baseEndpoint;

            return $"{baseEndpoint.TrimEnd('/')}/v1/traces";
        }

        /// <summary>
        /// Resolves the OTLP endpoint URL for metrics (/v1/metrics)
        /// Per OTLP specification, metrics are sent to /v1/metrics endpoint
        /// </summary>
        public string ResolveMetricsEndpoint()
        {
            var baseEndpoint = ResolveBaseEndpoint();
            if ((Otlp.Protocol ?? "grpc").Equals("grpc", StringComparison.OrdinalIgnoreCase))
                return baseEndpoint;

            return $"{baseEndpoint.TrimEnd('/')}/v1/metrics";
        }

        /// <summary>
        /// Resolves the OTLP endpoint URL specifically for logs (/v1/logs)
        /// Per OTLP specification, logs are sent to /v1/logs endpoint
        /// </summary>
        public string ResolveLogsEndpoint()
        {
            var baseEndpoint = ResolveBaseEndpoint();
            if ((Otlp.Protocol ?? "grpc").Equals("grpc", StringComparison.OrdinalIgnoreCase))
                return baseEndpoint;

            return $"{baseEndpoint.TrimEnd('/')}/v1/logs";
        }

        /// <summary>
        /// Resolves the OTLP endpoint URL for backward compatibility
        /// Returns base endpoint without /v1/* paths
        /// Note: Prefer specific methods (ResolveTracesEndpoint, ResolveMetricsEndpoint, ResolveLogsEndpoint)
        /// </summary>
        public string ResolveEndpoint()
        {
            return ResolveBaseEndpoint();
        }

        public sealed class OtlpSettings
        {
            /// <summary>
            /// OTLP base endpoint URL (default: http://localhost:4317)
            /// This is the base endpoint used by all telemetry type resolvers
            /// which append their specific /v1/* paths (/v1/traces, /v1/metrics, /v1/logs)
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
