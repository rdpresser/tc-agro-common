namespace TC.Agro.SharedKernel.Application.Ports
{
    public interface ICachedQuery<TResponse> : IBaseQuery<TResponse>, ICachedQuery;

    public interface ICachedQuery
    {
        string GetCacheKey { get; }
        void SetCacheKey(string cacheKey);

        TimeSpan? Duration { get; }
        TimeSpan? DistributedCacheDuration { get; }

        IReadOnlyCollection<string> CacheTags { get; }
    }

    public interface IInvalidateCache
    {
        IReadOnlyCollection<string> CacheTags { get; }
    }
}
