namespace TC.Agro.SharedKernel.Infrastructure.Authentication
{
    public sealed class JwtHelper
    {
        private const string JwtSectionName = "Auth:Jwt";

        // --------------------------------------------------
        // JWT configuration loaded from appsettings and environment variables
        // --------------------------------------------------
        public JwtOptions JwtSettings { get; }

        public JwtHelper(IConfiguration configuration)
        {
            // Bind section "Auth:Jwt" → JwtOptions
            JwtSettings = configuration.GetSection(JwtSectionName).Get<JwtOptions>()
                          ?? throw new InvalidOperationException($"JWT configuration section '{JwtSectionName}' is missing or invalid.");

            // Validate required settings
            ValidateSettings();
        }

        // --------------------------------------------------
        // Static convenience method to get configured JWT options
        // --------------------------------------------------
        public static JwtOptions Build(IConfiguration configuration) =>
            new JwtHelper(configuration).JwtSettings;

        // --------------------------------------------------
        // Validate JWT settings to ensure all required values are present
        // --------------------------------------------------
        private void ValidateSettings()
        {
            if (string.IsNullOrWhiteSpace(JwtSettings.SecretKey))
                throw new InvalidOperationException("JWT SecretKey is required but not configured.");

            if (JwtSettings.SecretKey.Length < 32)
                throw new InvalidOperationException("JWT SecretKey must be at least 32 characters long for security.");

            if (string.IsNullOrWhiteSpace(JwtSettings.Issuer))
                throw new InvalidOperationException("JWT Issuer is required but not configured.");

            if (JwtSettings.Audience == null || JwtSettings.Audience.Length == 0)
                throw new InvalidOperationException("JWT Audience is required but not configured.");

            if (JwtSettings.ExpirationInMinutes <= 0)
                throw new InvalidOperationException("JWT ExpirationInMinutes must be greater than 0.");
        }

        // --------------------------------------------------
        // Helper method to get expiration TimeSpan
        // --------------------------------------------------
        public TimeSpan GetExpirationTimeSpan() => TimeSpan.FromMinutes(JwtSettings.ExpirationInMinutes);

        // --------------------------------------------------
        // Helper method to get expiration DateTime
        // --------------------------------------------------
        public DateTimeOffset GetExpirationDateTime() => DateTimeOffset.UtcNow.Add(GetExpirationTimeSpan());
    }
}
