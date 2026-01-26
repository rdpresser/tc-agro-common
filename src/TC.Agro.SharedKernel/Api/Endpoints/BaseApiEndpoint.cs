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
                await Send.NotFoundAsync(ct);
                return;
            }

            if (response.IsUnauthorized())
            {
                await Send.UnauthorizedAsync(ct);
                return;
            }

            await Send.ErrorsAsync((int)HttpStatusCode.BadRequest, ct);
        }
    }
}