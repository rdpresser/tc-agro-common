namespace TC.Agro.SharedKernel.Api.Endpoints
{
    public abstract class BaseApiEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
        where TRequest : notnull
        where TResponse : class
    {
        protected async Task MatchResultAsync(Result<TResponse> response, CancellationToken ct = default)
        {
            if (response.IsSuccess)
            {
                await Send.OkAsync(response.Value, cancellation: ct);
                return;
            }

            if (response.IsNotFound())
            {
                var errors = response.Errors?.Select(e => new
                {
                    name = "NotFound",
                    reason = e,
                    code = "NotFound"
                }).ToArray();

                var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.4",
                    Title = "Not Found",
                    Instance = HttpContext?.Request.Path.Value ?? string.Empty
                };

                problemDetails.Extensions["traceId"] = HttpContext?.TraceIdentifier;

                if (errors is { Length: > 0 })
                    problemDetails.Extensions["errors"] = errors;

                await HttpContext!.Response.SendAsync(problemDetails, (int)HttpStatusCode.NotFound, cancellation: ct).ConfigureAwait(false);
                return;
            }

            if (response.IsUnauthorized())
            {
                var errors = response.Errors?.Select(e => new
                {
                    name = "Unauthorized",
                    reason = e,
                    code = "Unauthorized"
                }).ToArray();

                var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
                {
                    Status = (int)HttpStatusCode.Unauthorized,
                    Type = "https://www.rfc-editor.org/rfc/rfc7235#section-3.1",
                    Title = "Unauthorized",
                    Instance = HttpContext?.Request.Path.Value ?? string.Empty
                };

                problemDetails.Extensions["traceId"] = HttpContext?.TraceIdentifier;

                if (errors is { Length: > 0 })
                    problemDetails.Extensions["errors"] = errors;

                await HttpContext!.Response.SendAsync(problemDetails, (int)HttpStatusCode.Unauthorized, cancellation: ct).ConfigureAwait(false);
                return;
            }

            await Send.ErrorsAsync((int)HttpStatusCode.BadRequest, ct);
        }
    }
}