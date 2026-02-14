namespace TC.Agro.SharedKernel.Application.Behaviors
{
    [ExcludeFromCodeCoverage]
    public sealed class QueryCachingPostProcessorBehavior<TQuery, TResponse> : IPostProcessor<TQuery, TResponse>
        where TQuery : ICachedQuery<TResponse>
        where TResponse : class
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<QueryCachingPostProcessorBehavior<TQuery, TResponse>> _logger;
        private const string correlationIdHeader = "x-cache-correlation-id";

        public QueryCachingPostProcessorBehavior(ICacheService cacheService,
            ILogger<QueryCachingPostProcessorBehavior<TQuery, TResponse>> logger)
        {
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PostProcessAsync(IPostProcessorContext<TQuery, TResponse> context, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(context);

            // Early exit if Request is null (can happen during exception scenarios or failed pre-processing)
            if (context.Request is null)
            {
                return;
            }

            if (context.HttpContext.Request.Headers.TryGetValue(correlationIdHeader, out var cachedInfo) && cachedInfo == GenerateCacheKey(context))
            {
                context.HttpContext.Request.Headers.Remove(correlationIdHeader);
                return;
            }

            var genericType = _logger.GetType().GenericTypeArguments.FirstOrDefault()?.Name ?? "Unknown";
            var name = context.Request?.GetType().Name
                       ?? genericType;

            if (!context.HasValidationFailures)
            {
                await _cacheService.SetAsync
                (
                    GenerateCacheKey(context),
                    context.Response,
                    context.Request!.Duration,
                    context.Request.DistributedCacheDuration,
                    context.Request.CacheTags,
                    ct
                ).ConfigureAwait(false);

                var responseValues = new
                {
                    context.Request,
                    context.Response,
                };

                using (LogContext.PushProperty("ResponseContent", responseValues, true))
                {
                    _logger.LogInformation("Post-processing Request {Request} executed successfully", name);
                }
            }
            else
            {
                var responseValues = new
                {
                    context.Request,
                    context.Response,
                    Error = context.ValidationFailures
                };

                using (LogContext.PushProperty("ResponseContent", responseValues, true))
                {
                    _logger.LogError("Post-processing Request {Request} validation failed with error", name);
                }
            }

            return;
        }

        private static string GenerateCacheKey(IPostProcessorContext<TQuery, TResponse> context)
        {
            var _userContext = context.HttpContext.RequestServices.GetRequiredService<IUserContext>();

            // If you need to handle unauthenticated users, check IsAuthenticated property
            if (!_userContext.IsAuthenticated)
            {
                context.Request!.SetCacheKey("-anonymous-anonymous");
            }
            else
            {
                context.Request!.SetCacheKey($"-{_userContext.Id}-{_userContext.Username}");
            }

            return context.Request.GetCacheKey;
        }
    }
}
