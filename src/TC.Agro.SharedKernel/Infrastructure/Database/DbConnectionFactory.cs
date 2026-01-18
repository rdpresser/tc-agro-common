namespace TC.Agro.SharedKernel.Infrastructure.Database
{
    public sealed class DbConnectionFactory
    {
        private readonly PostgresOptions _options;

        public DbConnectionFactory(IOptions<PostgresOptions> options)
        {
            _options = options.Value;
        }

        // Strings
        public string ConnectionString => _options.ConnectionString;
        public string MaintenanceConnectionString => _options.MaintenanceConnectionString;
    }
}
