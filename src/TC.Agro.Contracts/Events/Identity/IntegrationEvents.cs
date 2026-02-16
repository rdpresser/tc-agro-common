namespace TC.Agro.Contracts.Events.Identity
{
    /// <summary>
    /// Integration event triggered when a new user is created.
    /// Exposes only the necessary information for other services,
    /// avoiding sensitive domain data like passwords or internal value objects.
    /// </summary>
    /// <remarks>
    /// Inherits from <see cref="BaseIntegrationEvent"/> to provide:
    /// - EventId: unique identifier for this event instance
    /// - AggregateId: identifier of the originating aggregate
    /// - OccurredOn: timestamp when the event occurred
    /// 
    /// - EventName: event name for logging and auditing
    /// 
    /// Naming convention: UserCreatedIntegrationEvent
    /// Used by Application layer to publish to the message bus for other services to consume.
    /// </remarks>
    public record UserCreatedIntegrationEvent(
        Guid OwnerId,
        string Name,
        string Email,
        string Username,
        string Role,
        DateTimeOffset OccurredOn
    ) : BaseIntegrationEvent(Guid.NewGuid(), OwnerId, OccurredOn, nameof(UserCreatedIntegrationEvent));

    /// <summary>
    /// Integration event triggered when an existing user's details are updated.
    /// Only essential user information is exposed for other services.
    /// </summary>
    /// <remarks>
    /// Inherits from <see cref="BaseIntegrationEvent"/> to maintain consistency and traceability.
    /// Naming convention: UserUpdatedIntegrationEvent
    /// </remarks>
    public record UserUpdatedIntegrationEvent(
        Guid OwnerId,
        string Name,
        string Email,
        string Username,
        DateTimeOffset OccurredOn
    ) : BaseIntegrationEvent(Guid.NewGuid(), OwnerId, OccurredOn, nameof(UserUpdatedIntegrationEvent));

    /// <summary>
    /// Integration event triggered when a user is deactivated.
    /// Contains only the user identifier and deactivation timestamp.
    /// </summary>
    /// <remarks>
    /// Inherits from <see cref="BaseIntegrationEvent"/>.
    /// Naming convention: UserDeactivatedIntegrationEvent
    /// </remarks>
    public record UserDeactivatedIntegrationEvent(
        Guid OwnerId,
        DateTimeOffset OccurredOn
    ) : BaseIntegrationEvent(Guid.NewGuid(), OwnerId, OccurredOn, nameof(UserDeactivatedIntegrationEvent));
}
