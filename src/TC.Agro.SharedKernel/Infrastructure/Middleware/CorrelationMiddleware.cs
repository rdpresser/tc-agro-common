namespace TC.Agro.SharedKernel.Infrastructure.Middleware
{
    public class CorrelationMiddleware
    {
        private readonly RequestDelegate _next;
        // Use consistent header naming across the application
        private const string _correlationIdHeader = "X-Correlation-ID";

        public CorrelationMiddleware(RequestDelegate next) => _next = next ?? throw new ArgumentNullException(nameof(next));

        public async Task Invoke(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(correlationIdGenerator);

            var correlationId = GetCorrelationId(context, correlationIdGenerator);
            context.TraceIdentifier = correlationId.ToString();
            AddCorrelationIdHeaderToResponse(context, correlationId);

            using (LogContext.PushProperty("CorrelationId", correlationId.ToString()))
            {
                await _next(context).ConfigureAwait(false);
            }
        }

        private static StringValues GetCorrelationId(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
        {
            if (context.Request.Headers.TryGetValue(_correlationIdHeader, out var correlationId) &&
                !string.IsNullOrWhiteSpace(correlationId.ToString()))
            {
                correlationIdGenerator.SetCorrelationId(correlationId.ToString());
                return correlationId;
            }
            else
            {
                var generatedCorrelationId = context.TraceIdentifier ?? Guid.NewGuid().ToString();
                correlationIdGenerator.SetCorrelationId(generatedCorrelationId);
                return correlationIdGenerator.CorrelationId;
            }
        }

        private static void AddCorrelationIdHeaderToResponse(HttpContext context, StringValues correlationId)
       => context.Response.OnStarting(() =>
       {
           context.Response.Headers[_correlationIdHeader] = new[] { correlationId.ToString() };
           return Task.CompletedTask;
       });
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CorrelationMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorrelationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorrelationMiddleware>();
        }
    }
}
