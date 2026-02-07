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

        public string ConnectionString => BuildConnectionString(Database);

        public string MaintenanceConnectionString => BuildConnectionString(MaintenanceDatabase);

        private string BuildConnectionString(string databaseName)
        {
            var baseConnection = $"Host={Host};Port={Port};Database={databaseName};Username={UserName};Password={Password};SearchPath={Schema};Timeout={ConnectionTimeout};CommandTimeout={ConnectionTimeout};Pooling=true;Minimum Pool Size={MinPoolSize};Maximum Pool Size={MaxPoolSize}";

            var parts = new List<string> { baseConnection };

            // Add SSL configuration if specified
            if (!string.IsNullOrWhiteSpace(SslMode))
                parts.Add($"SSL Mode={SslMode}");

            if (TrustServerCertificate.HasValue)
                parts.Add($"Trust Server Certificate={TrustServerCertificate.Value}");

            return string.Join(";", parts);
        }
    }
}
