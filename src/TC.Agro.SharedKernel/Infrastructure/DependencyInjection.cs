using TC.Agro.SharedKernel.Infrastructure.Clock;
using TC.Agro.SharedKernel.Infrastructure.Telemetry;

namespace TC.Agro.SharedKernel.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAgroInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<ICacheProvider, CacheProvider>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<DbConnectionFactory>();
            services.AddSingleton<ITokenProvider, TokenProvider>();
            services.AddScoped<IUserContext, UserContext>();

            // ============================================
            // PostgreSQL Configuration with Validation
            // ============================================
            services.AddOptions<PostgresOptions>()
                .Bind(configuration.GetSection("Database:Postgres"))
                .Validate(o => !string.IsNullOrWhiteSpace(o.Host),
                    "Database:Postgres:Host is required")
                .Validate(o => o.Port > 0 && o.Port < 65536,
                    "Database:Postgres:Port must be between 1-65535")
                .Validate(o => !string.IsNullOrWhiteSpace(o.Database),
                    "Database:Postgres:Database is required")
                .Validate(o => !string.IsNullOrWhiteSpace(o.UserName),
                    "Database:Postgres:UserName is required")
                .Validate(o => !string.IsNullOrWhiteSpace(o.Password),
                    "Database:Postgres:Password is required")
                .Validate(o => !string.IsNullOrWhiteSpace(o.Schema),
                    "Database:Postgres:Schema is required")
                .Validate(o => o.ConnectionTimeout > 0,
                    "Database:Postgres:ConnectionTimeout must be > 0")
                .Validate(o => o.MinPoolSize > 0,
                    "Database:Postgres:MinPoolSize must be > 0")
                .Validate(o => o.MaxPoolSize >= o.MinPoolSize,
                    "Database:Postgres:MaxPoolSize must be >= MinPoolSize")
                .ValidateOnStart();

            // ============================================
            // Redis Cache Configuration with Validation
            // ============================================
            services.AddOptions<RedisOptions>()
                .Bind(configuration.GetSection("Cache:Redis"))
                .Validate(o => !string.IsNullOrWhiteSpace(o.Host),
                    "Cache:Redis:Host is required")
                .Validate(o => o.Port > 0 && o.Port < 65536,
                    "Cache:Redis:Port must be between 1-65535")
                .Validate(o => !string.IsNullOrWhiteSpace(o.InstanceName),
                    "Cache:Redis:InstanceName is required")
                .Validate(o => !(o.Secure && string.IsNullOrWhiteSpace(o.Password)),
                    "Cache:Redis:Password is required when Secure=true")
                .ValidateOnStart();

            // ============================================
            // RabbitMQ Messaging Configuration with Validation
            // ============================================
            services.AddOptions<RabbitMqOptions>()
                .Bind(configuration.GetSection("Messaging:RabbitMQ"))
                .Validate(o => !string.IsNullOrWhiteSpace(o.Host),
                    "Messaging:RabbitMQ:Host is required")
                .Validate(o => o.Port > 0 && o.Port < 65536,
                    "Messaging:RabbitMQ:Port must be between 1-65535")
                .Validate(o => o.ManagementPort > 0 && o.ManagementPort < 65536,
                    "Messaging:RabbitMQ:ManagementPort must be between 1-65535")
                .Validate(o => !string.IsNullOrWhiteSpace(o.VirtualHost),
                    "Messaging:RabbitMQ:VirtualHost is required")
                .Validate(o => !string.IsNullOrWhiteSpace(o.UserName),
                    "Messaging:RabbitMQ:UserName is required")
                .Validate(o => !string.IsNullOrWhiteSpace(o.Password),
                    "Messaging:RabbitMQ:Password is required")
                .Validate(o => !string.IsNullOrWhiteSpace(o.Exchange),
                    "Messaging:RabbitMQ:Exchange is required")
                .Validate(o => o.Port != o.ManagementPort,
                    "Messaging:RabbitMQ:Port and ManagementPort must be different")
                .ValidateOnStart();

            // ============================================
            // Grafana/OpenTelemetry Configuration with Validation
            // ============================================
            services.AddOptions<GrafanaOptions>()
                .Bind(configuration.GetSection("Telemetry:Grafana"))
                .Validate(o => !string.IsNullOrWhiteSpace(o.Agent.Host),
                    "Telemetry:Grafana:Agent:Host is required")
                .Validate(o => o.Agent.OtlpGrpcPort > 0 && o.Agent.OtlpGrpcPort < 65536,
                    "Telemetry:Grafana:Agent:OtlpGrpcPort must be between 1-65535")
                .Validate(o => o.Agent.OtlpHttpPort > 0 && o.Agent.OtlpHttpPort < 65536,
                    "Telemetry:Grafana:Agent:OtlpHttpPort must be between 1-65535")
                .Validate(o => o.Agent.MetricsPort > 0 && o.Agent.MetricsPort < 65536,
                    "Telemetry:Grafana:Agent:MetricsPort must be between 1-65535")
                .Validate(o => o.Agent.OtlpGrpcPort != o.Agent.OtlpHttpPort,
                    "Telemetry:Grafana:Agent:OtlpGrpcPort and OtlpHttpPort must be different")
                .Validate(o => !string.IsNullOrWhiteSpace(o.Otlp.Endpoint),
                    "Telemetry:Grafana:Otlp:Endpoint is required")
                .Validate(o => IsValidUri(o.Otlp.Endpoint),
                    "Telemetry:Grafana:Otlp:Endpoint must be a valid URI")
                .Validate(o => !string.IsNullOrWhiteSpace(o.Otlp.Protocol),
                    "Telemetry:Grafana:Otlp:Protocol is required")
                .Validate(o => o.Otlp.Protocol.Equals("grpc", StringComparison.OrdinalIgnoreCase)
                    || o.Otlp.Protocol.Equals("http/protobuf", StringComparison.OrdinalIgnoreCase),
                    "Telemetry:Grafana:Otlp:Protocol must be 'grpc' or 'http/protobuf'")
                .Validate(o => o.Otlp.TimeoutSeconds > 0,
                    "Telemetry:Grafana:Otlp:TimeoutSeconds must be > 0")
                .ValidateOnStart();

            // ============================================
            // JWT Authentication Configuration with Validation
            // ============================================
            services.AddOptions<JwtOptions>()
                .Bind(configuration.GetSection("Auth:Jwt"))
                .Validate(o => !string.IsNullOrWhiteSpace(o.SecretKey),
                    "Auth:Jwt:SecretKey is required")
                .Validate(o => o.SecretKey.Length >= 32,
                    "Auth:Jwt:SecretKey must be at least 32 characters (256 bits)")
                .Validate(o => !string.IsNullOrWhiteSpace(o.Issuer),
                    "Auth:Jwt:Issuer is required")
                .Validate(o => IsValidIssuer(o.Issuer),
                    "Auth:Jwt:Issuer must be a valid URI or service identifier (e.g. http://localhost:5001 or tc-agro-identity-service)")
                .Validate(o => o.Audience != null && o.Audience.Length > 0,
                    "Auth:Jwt:Audience must contain at least one audience")
                .Validate(o => o.Audience?.All(a => !string.IsNullOrWhiteSpace(a)) ?? false,
                    "Auth:Jwt:Audience cannot contain empty values")
                .Validate(o => o.ExpirationInMinutes > 0,
                    "Auth:Jwt:ExpirationInMinutes must be > 0")
                .Validate(o => o.ExpirationInMinutes <= 43200,
                    "Auth:Jwt:ExpirationInMinutes must not exceed 30 days (43200 minutes)")
                .ValidateOnStart();

            return services;
        }

        /// <summary>
        /// Helper method to validate URI format without Uri.TryParse (compatible with all .NET versions)
        /// </summary>
        private static bool IsValidUri(string? uriString)
        {
            if (string.IsNullOrWhiteSpace(uriString))
                return false;

            try
            {
                _ = new Uri(uriString, UriKind.Absolute);
                return true;
            }
            catch (UriFormatException)
            {
                return false;
            }
        }

        private static bool IsValidIssuer(string? issuer)
        {
            if (string.IsNullOrWhiteSpace(issuer))
                return false;

            if (IsValidUri(issuer))
                return true;

            return issuer.All(ch => char.IsLetterOrDigit(ch) || ch is '-' or '_' or '.');
        }
    }
}
