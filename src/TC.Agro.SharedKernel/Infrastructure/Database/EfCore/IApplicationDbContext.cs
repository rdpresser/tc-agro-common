using Microsoft.EntityFrameworkCore;

namespace TC.Agro.SharedKernel.Infrastructure.Database.EfCore
{
    /// <summary>
    /// Marker interface for DbContext implementations that support UnitOfWork pattern.
    /// Used to enable generic repository and outbox patterns without coupling to specific DbContext types.
    /// </summary>
    public interface IApplicationDbContext : IUnitOfWork
    {
        /// <summary>
        /// Gets the underlying DbContext for advanced scenarios.
        /// </summary>
        DbContext DbContext { get; }

        /// <summary>
        /// Gets a DbSet for the specified entity type.
        /// </summary>
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}
