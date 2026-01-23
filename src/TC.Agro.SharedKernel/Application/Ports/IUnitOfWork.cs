namespace TC.Agro.SharedKernel.Application.Ports
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
