using TC.Agro.SharedKernel.Api.Endpoints;
using TC.Agro.SharedKernel.Application.Behaviors;

namespace TC.Agro.SharedKernel.Api.Extensions
{
    public static class EndpointCachingExtensions
    {
        private const string TestingEnvironment = "Testing";

        public static void AddCacheInvalidationIfNotTesting<TRequest, TResponse>(
            this BaseApiEndpoint<TRequest, TResponse> endpoint)
            where TRequest : IBaseCommand<TResponse>
            where TResponse : class
        {
            ArgumentNullException.ThrowIfNull(endpoint);

            if (IsTestingEnvironment())
            {
                return;
            }

            endpoint.PostProcessor<CacheInvalidationPostProcessorBehavior<TRequest, TResponse>>();
        }

        public static void AddQueryCachingIfNotTesting<TQuery, TResponse>(
            this BaseApiEndpoint<TQuery, TResponse> endpoint)
            where TQuery : ICachedQuery<TResponse>
            where TResponse : class
        {
            ArgumentNullException.ThrowIfNull(endpoint);

            if (IsTestingEnvironment())
            {
                return;
            }

            endpoint.PreProcessor<QueryCachingPreProcessorBehavior<TQuery, TResponse>>();
            endpoint.PostProcessor<QueryCachingPostProcessorBehavior<TQuery, TResponse>>();
        }

        private static bool IsTestingEnvironment() =>
            string.Equals(
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                TestingEnvironment,
                StringComparison.OrdinalIgnoreCase);
    }
}
