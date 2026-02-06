namespace TC.Agro.SharedKernel.Infrastructure.Database
{
    public sealed class PostgresHelper
    {
        private const string PostgresSectionName = "Database:Postgres";
        public PostgresOptions PostgresSettings { get; }

        public PostgresHelper(IConfiguration configuration)
        {
            // Bind section Database → PostgresOptions
            PostgresSettings = configuration.GetSection(PostgresSectionName).Get<PostgresOptions>()
                               ?? new PostgresOptions();
        }

        // Static convenience method
        public static PostgresOptions Build(IConfiguration configuration) =>
            new PostgresHelper(configuration).PostgresSettings;
    }
}
