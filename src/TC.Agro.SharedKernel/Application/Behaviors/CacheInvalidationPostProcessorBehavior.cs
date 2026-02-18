namespace TC.Agro.SharedKernel.Application.Behaviors
{
    [ExcludeFromCodeCoverage]
    public sealed class CacheInvalidationPostProcessorBehavior<TRequest, TResponse> : IPostProcessor<TRequest, TResponse>
        where TRequest : IBaseCommand<TResponse>
        where TResponse : class
    {
        private readonly ICacheService _cacheService;

        public CacheInvalidationPostProcessorBehavior(ICacheService cacheService)
        {
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task PostProcessAsync(IPostProcessorContext<TRequest, TResponse> context, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.HasValidationFailures)
            {
                return;
            }

            if (context.Request is not IInvalidateCache invalidation || invalidation.CacheTags.Count == 0)
            {
                return;
            }

            var tags = invalidation.CacheTags
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase);

            await _cacheService.RemoveByTagAsync(tags, ct).ConfigureAwait(false);
        }
    }
}