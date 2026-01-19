using TC.Agro.SharedKernel.Infrastructure.Clock;

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

            services.AddOptions<PostgresOptions>()
                .Bind(configuration.GetSection("Database:Postgres"))
                .Validate(o => !string.IsNullOrWhiteSpace(o.Host), "Database:Postgres:Host é obrigatório")
                .Validate(o => o.Port > 0, "Database:Postgres:Port inválida")
                .Validate(o => !string.IsNullOrWhiteSpace(o.Database), "Database:Postgres:Database é obrigatório")
                .ValidateOnStart();

            services.AddOptions<RedisOptions>()
                .Bind(configuration.GetSection("Cache:Redis"))
                .ValidateOnStart();

            services.AddOptions<RabbitMqOptions>()
                .Bind(configuration.GetSection("Messaging:RabbitMQ"))
                .ValidateOnStart();

            services.AddOptions<JwtOptions>()
                .Bind(configuration.GetSection("Auth:Jwt"))
                .ValidateOnStart();

            return services;
        }
    }
}
