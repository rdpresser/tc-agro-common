namespace TC.Agro.SharedKernel.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCorrelationIdGenerator(this IServiceCollection services)
        {
            services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

            return services;
        }

        /// <summary>
        /// Loads environment variables from .env files
        /// </summary>
        /// <param name="environment">The hosting environment</param>
        public static void LoadEnvironmentFiles(IHostEnvironment environment)
        {
            var environmentName = environment.EnvironmentName.ToLowerInvariant();

            // Find project root by looking for solution file or git directory
            var projectRoot = FindProjectRoot() ?? Directory.GetCurrentDirectory();

            var logger = CreateBootstrapLogger();

            // Load base .env file first (if exists)
            var baseEnvFile = Path.Combine(projectRoot, ".env");
            if (File.Exists(baseEnvFile))
            {
                DotNetEnv.Env.Load(baseEnvFile);
                logger?.LogInformation("Loaded base .env from: {EnvFile}", baseEnvFile);
                Console.WriteLine($"Loaded base .env from: {baseEnvFile}");
            }

            // Load environment-specific .env file (overrides base values)
            var envFile = Path.Combine(projectRoot, $".env.{environmentName}");
            if (File.Exists(envFile))
            {
                DotNetEnv.Env.Load(envFile);
                logger?.LogInformation("Loaded {Environment} .env from: {EnvFile}", environmentName, envFile);
                Console.WriteLine($"Loaded {environmentName} .env from: {envFile}");
            }
            else
            {
                logger?.LogWarning("Environment file not found: {EnvFile}", envFile);
                Console.WriteLine($"Environment file not found: {envFile}");
            }
        }

        /// <summary>
        /// Finds the project root directory by looking for solution file or git directory
        /// </summary>
        /// <returns>The project root directory path or null if not found</returns>
        private static string? FindProjectRoot()
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());

            while (directory != null)
            {
                // Look for common project root indicators
                if (directory.GetFiles("*.sln").Length > 0 ||
                    directory.GetFiles("*.slnx").Length > 0 ||
                    directory.GetDirectories(".git").Length > 0 ||
                    HasEnvFiles(directory))
                {
                    return directory.FullName;
                }
                directory = directory.Parent;
            }

            return null;
        }

        /// <summary>
        /// Checks if a directory contains .env files
        /// </summary>
        /// <param name="directory">The directory to check</param>
        /// <returns>True if .env files are found, false otherwise</returns>
        private static bool HasEnvFiles(DirectoryInfo directory)
        {
            return directory.GetFiles(".env").Length > 0 ||
                   directory.GetFiles(".env.*").Length > 0;
        }

        /// <summary>
        /// Creates a bootstrap logger for early logging before DI container is built
        /// </summary>
        /// <returns>A logger instance or null if creation fails</returns>
        private static ILogger? CreateBootstrapLogger()
        {
            try
            {
                using var loggerFactory = LoggerFactory.Create(builder =>
                    builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
                return loggerFactory.CreateLogger("EnvironmentConfiguration");
            }
            catch
            {
                // If logger creation fails, return null and fall back to Console.WriteLine
                return null;
            }
        }
    }
}
