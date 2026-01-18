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

        public string ConnectionString =>
            $"Host={Host};Port={Port};Database={Database};Username={UserName};Password={Password};SearchPath={Schema};Timeout={ConnectionTimeout};CommandTimeout={ConnectionTimeout};Pooling=true;Minimum Pool Size={MinPoolSize};Maximum Pool Size={MaxPoolSize}";

        public string MaintenanceConnectionString =>
            $"Host={Host};Port={Port};Database={MaintenanceDatabase};Username={UserName};Password={Password};Timeout={ConnectionTimeout};CommandTimeout={ConnectionTimeout};Pooling=true;Minimum Pool Size={MinPoolSize};Maximum Pool Size={MaxPoolSize}";
    }
}
