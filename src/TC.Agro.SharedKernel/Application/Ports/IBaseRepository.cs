namespace TC.Agro.SharedKernel.Application.Ports
{
    /// <summary>
    /// Generic abstraction for repositories responsible for persisting aggregate roots.
    /// Defines a contract that hides persistence details (Marten, EF, etc.) 
    /// while exposing consistent operations for aggregates.
    /// </summary>
    public interface IBaseRepository<TAggregate> where TAggregate : BaseAggregateRoot
    {
        /// <summary>
        /// Retrieves an aggregate by its unique identifier. 
        /// Returns null if the aggregate does not exist.
        /// </summary>
        Task<TAggregate?> GetByIdAsync(Guid aggregateId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds the aggregate state (including new domain events if any).
        /// </summary>
        void Add(TAggregate aggregate);

        /// <summary>
        /// Adds a collection of aggregate items to the repository in a single operation.
        /// </summary>
        /// <remarks>Using this method to add multiple aggregates at once can improve performance compared
        /// to adding them individually. The collection should not be modified during the operation.</remarks>
        /// <param name="aggregates">The collection of aggregate items to add. This parameter cannot be null and must contain valid aggregate
        /// instances.</param>
        void AddRange(IEnumerable<TAggregate> aggregates);
        /// Updates an existing aggregate with new state changes.
        /// </summary>
        Task UpdateAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

        /////// <summary>
        /////// Retrieves all aggregates of this type.
        /////// Use with caution as this may be expensive for large datasets.
        /////// </summary>
        ////Task<IEnumerable<TAggregate>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Permanently deletes the aggregate identified by its ID.
        /// </summary>
        Task DeleteAsync(Guid aggregateId, CancellationToken cancellationToken = default);
    }
}