namespace TC.Agro.SharedKernel.Api.Extensions
{
    /// <summary>
    /// Extension methods for Prometheus/Grafana metrics authentication.
    /// </summary>
    public static class MetricsAuthenticationExtensions
    {
        /// <summary>
        /// Adds authentication middleware for the /metrics endpoint using Bearer token validation.
        /// Token is read from GRAFANA_OTEL_PROMETHEUS_API_TOKEN environment variable.
        /// </summary>
        public static IApplicationBuilder UseMetricsAuthentication(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/metrics")
                {
                    var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
                    if (authHeader?.StartsWith("Bearer ") == true)
                    {
                        var token = authHeader["Bearer ".Length..].Trim();
                        var expectedToken = Environment.GetEnvironmentVariable("GRAFANA_OTEL_PROMETHEUS_API_TOKEN");

                        if (token == expectedToken)
                        {
                            await next().ConfigureAwait(false);
                            return;
                        }
                    }

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized").ConfigureAwait(false);
                    return;
                }

                await next().ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Adds authentication middleware for the /metrics endpoint with custom token provider.
        /// </summary>
        public static IApplicationBuilder UseMetricsAuthentication(this IApplicationBuilder app, Func<string?> tokenProvider)
        {
            return app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/metrics")
                {
                    var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
                    if (authHeader?.StartsWith("Bearer ") == true)
                    {
                        var token = authHeader["Bearer ".Length..].Trim();
                        var expectedToken = tokenProvider();

                        if (!string.IsNullOrEmpty(expectedToken) && token == expectedToken)
                        {
                            await next().ConfigureAwait(false);
                            return;
                        }
                    }

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized").ConfigureAwait(false);
                    return;
                }

                await next().ConfigureAwait(false);
            });
        }
    }
}
