namespace TC.Agro.SharedKernel.Application.Handlers;

public abstract class BaseCommandHandler<TCommand, TResponse, TAggregate, TRepository>
    : BaseHandler<TCommand, TResponse>
    where TCommand : IBaseCommand<TResponse>
    where TResponse : class
    where TAggregate : BaseAggregateRoot
    where TRepository : IBaseRepository<TAggregate>
{
    protected TRepository Repository { get; }
    protected IUserContext UserContext { get; }
    protected IUnitOfWork UnitOfWork { get; }
    protected ILogger Logger { get; }

    protected BaseCommandHandler(
        TRepository repository,
        IUserContext userContext,
        IUnitOfWork unitOfWork,
        ILogger logger)
    {
        Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        UserContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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

        // 5) Commit (EF Core transaction + Wolverine Outbox flush happens after commit)
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

    protected virtual Task CommitAsync(CancellationToken ct)
        => UnitOfWork.SaveChangesAsync(ct);
}
