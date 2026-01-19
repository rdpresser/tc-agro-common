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
        Guid AggregateId,
        string Name,
        string Email,
        string Username,
        string Role,
        DateTimeOffset OccurredOn
    ) : BaseIntegrationEvent(Guid.NewGuid(), AggregateId, OccurredOn, nameof(UserCreatedIntegrationEvent));
}
