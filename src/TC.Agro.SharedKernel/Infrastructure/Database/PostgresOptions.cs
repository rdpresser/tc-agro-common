namespace TC.Agro.SharedKernel.Infrastructure.Database
{
    public sealed class PostgresOptions
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5432;
        public string Database { get; set; } = "tc-agro-default-db";
        public string MaintenanceDatabase { get; set; } = "postgres";
        public string UserName { get; set; } = "postgres";
        public string Password { get; set; } = "postgres";
        public string Schema { get; set; } = "public";
        public int ConnectionTimeout { get; set; } = 30;
        public int MinPoolSize { get; set; } = 2;
        public int MaxPoolSize { get; set; } = 20;

        /// <summary>
        /// SSL Mode for Supabase/Cloud databases (Disable, Allow, Prefer, Require, VerifyCA, VerifyFull)
        /// </summary>
        public string? SslMode { get; set; }

        /// <summary>
        /// Trust server certificate (use with caution in production)
        /// </summary>
        public bool? TrustServerCertificate { get; set; }

        public string ConnectionString
        {
            get
            {
                var builder = new System.Text.StringBuilder();
                builder.Append($"Host={Host};Port={Port};Database={Database};Username={UserName};Password={Password};SearchPath={Schema};Timeout={ConnectionTimeout};CommandTimeout={ConnectionTimeout};Pooling=true;Minimum Pool Size={MinPoolSize};Maximum Pool Size={MaxPoolSize}");

                // Add SSL configuration if specified
                if (!string.IsNullOrEmpty(SslMode))
                {
                    builder.Append($";SSL Mode={SslMode}");
                }

                if (TrustServerCertificate.HasValue)
                {
                    builder.Append($";Trust Server Certificate={TrustServerCertificate.Value}");
                }

                return builder.ToString();
            }
        }

        public string MaintenanceConnectionString
        {
            get
            {
                var builder = new System.Text.StringBuilder();
                builder.Append($"Host={Host};Port={Port};Database={MaintenanceDatabase};Username={UserName};Password={Password};Timeout={ConnectionTimeout};CommandTimeout={ConnectionTimeout};Pooling=true;Minimum Pool Size={MinPoolSize};Maximum Pool Size={MaxPoolSize}");

                // Add SSL configuration if specified
                if (!string.IsNullOrEmpty(SslMode))
                {
                    builder.Append($";SSL Mode={SslMode}");
                }

                if (TrustServerCertificate.HasValue)
                {
                    builder.Append($";Trust Server Certificate={TrustServerCertificate.Value}");
                }

                return builder.ToString();
            }
        }
    }
}
