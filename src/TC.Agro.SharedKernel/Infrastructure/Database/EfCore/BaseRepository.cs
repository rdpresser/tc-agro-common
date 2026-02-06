namespace TC.Agro.SharedKernel.Infrastructure.Database.EfCore
{
    /// <summary>
    /// Generic base repository for aggregate roots using EF Core.
    /// Provides standard CRUD operations with DbContext abstraction.
    /// </summary>
    /// <typeparam name="TAggregate">The aggregate root type.</typeparam>
    /// <typeparam name="TDbContext">The DbContext type that implements IApplicationDbContext.</typeparam>
    public abstract class BaseRepository<TAggregate, TDbContext> : IBaseRepository<TAggregate>
        where TAggregate : BaseAggregateRoot
        where TDbContext : DbContext, IApplicationDbContext
    {
        protected TDbContext DbContext { get; }
        protected DbSet<TAggregate> DbSet { get; }

        protected BaseRepository(TDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            DbSet = dbContext.Set<TAggregate>();
        }

        /// <inheritdoc />
        public virtual void Add(TAggregate aggregate)
        {
            DbSet.Add(aggregate);
        }

        /// <inheritdoc />
        public virtual void Delete(TAggregate aggregateRoot)
        {
            DbSet.Remove(aggregateRoot);
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(Guid aggregateId, CancellationToken cancellationToken = default)
        {
            var aggregate = await GetByIdAsync(aggregateId, cancellationToken).ConfigureAwait(false);
            if (aggregate != null)
            {
                Delete(aggregate);
            }
        }

        /// <inheritdoc />
        public virtual async Task<TAggregate?> GetByIdAsync(Guid aggregateId, CancellationToken cancellationToken = default)
        {
            return await DbSet.FindAsync([aggregateId], cancellationToken).ConfigureAwait(false);
        }
    }
}
