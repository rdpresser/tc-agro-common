namespace TC.Agro.SharedKernel.Application.Ports;

/// <summary>
/// Extends IUnitOfWork with transactional outbox capabilities for message publishing.
/// Used by command handlers that need to publish integration events atomically with data persistence.
/// </summary>
/// <remarks>
/// This interface enables the Transactional Outbox Pattern:
/// - Enqueue messages during command execution
/// - Commit EF Core changes and flush outbox messages in a single transaction
/// - Ensures atomicity between data persistence and message publishing
/// </remarks>
public interface ITransactionalOutbox : IUnitOfWork
{
    /// <summary>
    /// Enqueues a message to be published after the transaction commits.
    /// </summary>
    /// <typeparam name="T">The type of message to enqueue.</typeparam>
    /// <param name="message">The message to enqueue.</param>
    /// <param name="ct">Cancellation token.</param>
    ValueTask EnqueueAsync<T>(T message, CancellationToken ct = default);

    /// <summary>
    /// Enqueues multiple messages to be published after the transaction commits.
    /// </summary>
    /// <typeparam name="T">The type of messages to enqueue.</typeparam>
    /// <param name="messages">The messages to enqueue.</param>
    /// <param name="ct">Cancellation token.</param>
    ValueTask EnqueueAsync<T>(IReadOnlyCollection<T> messages, CancellationToken ct = default);
}
