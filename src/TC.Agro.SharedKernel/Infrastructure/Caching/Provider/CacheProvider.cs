namespace TC.Agro.SharedKernel.Infrastructure.Caching.Provider
{
    public sealed class CacheProvider : ICacheProvider
    {
        private readonly RedisOptions _options;

        public CacheProvider(IOptions<RedisOptions> options)
        {
            _options = options.Value;
        }

        public string InstanceName => _options.InstanceName;
        public string ConnectionString => _options.ConnectionString;
    }
}
