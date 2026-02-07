namespace TC.Agro.SharedKernel.Infrastructure.Database.EfCore
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationBuilderExtensions
    {
        // Applies pending migrations to the database
        public static async Task ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            await dbContext.DbContext.Database.MigrateAsync().ConfigureAwait(false);
        }
    }
}
