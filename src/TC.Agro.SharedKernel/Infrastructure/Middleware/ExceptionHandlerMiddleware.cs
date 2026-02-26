namespace TC.Agro.SharedKernel.Infrastructure.Middleware
{
    /// <summary>
    /// Middleware for handling exceptions globally and returning standardized ProblemDetails responses.
    /// </summary>
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (OperationCanceledException exception) when (context.RequestAborted.IsCancellationRequested)
            {
                _logger.LogDebug(
                    exception,
                    "Request cancelled by client. Method={Method} Path={Path} TraceId={TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.TraceIdentifier);

                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 499;
                }
            }
            catch (OperationCanceledException exception)
            {
                _logger.LogWarning(
                    exception,
                    "Operation cancelled before completion. Method={Method} Path={Path} TraceId={TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.TraceIdentifier);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

                if (context.Response.HasStarted)
                {
                    _logger.LogWarning(
                        "Response already started. Skipping ProblemDetails write. Method={Method} Path={Path} TraceId={TraceId}",
                        context.Request.Method,
                        context.Request.Path,
                        context.TraceIdentifier);

                    return;
                }

                var exceptionDetails = GetExceptionDetails(exception);

                var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
                {
                    Status = exceptionDetails.Status,
                    Type = exceptionDetails.Type,
                    Title = exceptionDetails.Title,
                    Detail = exceptionDetails.Detail
                };

                if (exceptionDetails.Errors is not null)
                {
                    problemDetails.Extensions["errors"] = exceptionDetails.Errors;
                }

                context.Response.StatusCode = exceptionDetails.Status;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsJsonAsync(problemDetails).ConfigureAwait(false);
            }
        }

        private static ExceptionDetails GetExceptionDetails(Exception exception)
        {
            return exception switch
            {
                ValidationException validationException => new ExceptionDetails(
                    StatusCodes.Status400BadRequest,
                    "ValidationFailure",
                    "Validation error",
                    "One or more validation errors occurred.",
                    validationException.Errors),

                _ => new ExceptionDetails(
                    StatusCodes.Status500InternalServerError,
                    "InternalServerError",
                    "An error occurred",
                    "An unexpected error occurred.",
                    null)
            };
        }

        internal record ExceptionDetails(
            int Status,
            string Type,
            string Title,
            string Detail,
            IEnumerable<object>? Errors);
    }

    /// <summary>
    /// Extension method for adding the ExceptionHandlerMiddleware to the request pipeline.
    /// </summary>
    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
