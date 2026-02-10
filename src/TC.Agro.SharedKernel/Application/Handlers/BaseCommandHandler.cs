namespace TC.Agro.SharedKernel.Application.Handlers;

/// <summary>
/// Base command handler for operations that modify aggregates and may publish integration events.
/// Uses ITransactionalOutbox for atomic persistence and message publishing.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <typeparam name="TAggregate">The aggregate root type.</typeparam>
/// <typeparam name="TRepository">The repository type.</typeparam>
public abstract class BaseCommandHandler<TCommand, TResponse, TAggregate, TRepository>
    : BaseHandler<TCommand, TResponse>
    where TCommand : IBaseCommand<TResponse>
    where TResponse : class
    where TAggregate : BaseAggregateRoot
    where TRepository : IBaseRepository<TAggregate>
{
    protected TRepository Repository { get; }
    protected IUserContext UserContext { get; }
    protected ITransactionalOutbox Outbox { get; }
    protected Microsoft.Extensions.Logging.ILogger Logger { get; }

    protected BaseCommandHandler(
        TRepository repository,
        IUserContext userContext,
        ITransactionalOutbox outbox,
        Microsoft.Extensions.Logging.ILogger logger)
    {
        Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        UserContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        Outbox = outbox ?? throw new ArgumentNullException(nameof(outbox));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public sealed override async Task<Result<TResponse>> ExecuteAsync(TCommand command, CancellationToken ct = default)
    {
        var operationId = Guid.NewGuid().ToString("N")[..8];

        Logger.LogInformation(
            "Starting operation {OperationId} for command {CommandName} by user {UserId}",
            operationId,
            typeof(TCommand).Name,
            UserContext.Id);

        // 1) Map
        var aggregateResult = await MapAsync(command, ct).ConfigureAwait(false);
        if (!aggregateResult.IsSuccess)
        {
            // ✅ Diferenciar NotFound de ValidationErrors
            if (aggregateResult.IsNotFound())
            {
                Logger.LogWarning(
                    "Operation {OperationId} failed: Resource not found for command {CommandName}",
                    operationId,
                    typeof(TCommand).Name);

                // ✅ Usar o padrão existente: AddError + BuildNotFoundResult()
                foreach (var error in aggregateResult.Errors ?? ["Resource not found"])
                {
                    AddError(error, "Resource not found", Severity.Warning);
                }
                return BuildNotFoundResult();
            }

            // ✅ ValidationErrors (BadRequest)
            AddErrors(aggregateResult.ValidationErrors);
            return BuildValidationErrorResult();
        }

        var aggregate = aggregateResult.Value;

        // 2) Validate
        var validation = await ValidateAsync(aggregate, ct).ConfigureAwait(false);
        if (!validation.IsSuccess)
        {
            AddErrors(validation.ValidationErrors);
            return BuildValidationErrorResult();
        }

        // 3) Persist
        await PersistAsync(aggregate, ct).ConfigureAwait(false);

        // 4) Publish integration events (Outbox)
        await PublishIntegrationEventsAsync(aggregate, ct).ConfigureAwait(false);

        // 5) Commit (EF Core transaction + Outbox flush in single transaction)
        await CommitAsync(ct).ConfigureAwait(false);

        // 6) Build response
        var response = await BuildResponseAsync(aggregate, ct).ConfigureAwait(false);

        Logger.LogInformation(
            "Operation {OperationId} completed successfully for command {CommandName}",
            operationId,
            typeof(TCommand).Name);

        return response;
    }

    // Mandatory
    protected abstract Task<Result<TAggregate>> MapAsync(TCommand command, CancellationToken ct);
    protected abstract Task<TResponse> BuildResponseAsync(TAggregate aggregate, CancellationToken ct);

    // Optional hooks
    protected virtual Task<Result> ValidateAsync(TAggregate aggregate, CancellationToken ct)
        => Task.FromResult(Result.Success());

    protected virtual Task PersistAsync(TAggregate aggregate, CancellationToken ct)
    {
        Repository.Add(aggregate);
        return Task.CompletedTask;
    }

    protected virtual Task PublishIntegrationEventsAsync(TAggregate aggregate, CancellationToken ct)
        => Task.CompletedTask;

    /// <summary>
    /// Commits EF Core changes and flushes outbox messages in a single transaction.
    /// </summary>
    protected virtual Task CommitAsync(CancellationToken ct)
        => Outbox.SaveChangesAsync(ct);
}
