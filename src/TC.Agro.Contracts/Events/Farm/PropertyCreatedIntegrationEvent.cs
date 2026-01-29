namespace TC.Agro.Contracts.Events.Farm
{
    /// <summary>
    /// Integration event raised when a property is created.
    /// Exposes information needed by other services for property awareness.
    /// </summary>
    /// <remarks>
    /// Inherits from <see cref="BaseIntegrationEvent"/> to provide:
    /// - EventId: unique identifier for this event instance
    /// - AggregateId: identifier of the originating Property aggregate
    /// - OccurredOn: timestamp when the event occurred
    /// - EventName: event name for logging and auditing
    /// </remarks>
    public sealed record PropertyCreatedIntegrationEvent(
        Guid PropertyId,
        string Name,
        string Address,
        string City,
        string State,
        string Country,
        double? Latitude,
        double? Longitude,
        double AreaHectares,
        Guid OwnerId,
        DateTimeOffset OccurredOn
    ) : BaseIntegrationEvent(Guid.NewGuid(), PropertyId, OccurredOn, nameof(PropertyCreatedIntegrationEvent));
}
