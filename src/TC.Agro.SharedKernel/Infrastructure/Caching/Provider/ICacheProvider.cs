namespace TC.Agro.SharedKernel.Infrastructure.Caching.Provider
{
    public interface ICacheProvider
    {
        string InstanceName { get; }
        string ConnectionString { get; }
    }
}
