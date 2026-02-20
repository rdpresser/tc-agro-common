namespace TC.Agro.Contracts.Events.Farm
{
    /// <summary>
    /// Integration event raised when a sensor is registered.
    /// Exposes information needed by other services for sensor awareness.
    /// </summary>
    /// <remarks>
    /// Inherits from <see cref="BaseIntegrationEvent"/> to provide:
    /// - EventId: unique identifier for this event instance
    /// - AggregateId: identifier of the originating Sensor aggregate
    /// - OccurredOn: timestamp when the event occurred
    /// - EventName: event name for logging and auditing
    /// 
    /// Includes RelatedIds with PlotId for cross-aggregate correlation.
    /// </remarks>
    public record SensorRegisteredIntegrationEvent(
        Guid SensorId,
        Guid OwnerId,
        Guid PropertyId,
        Guid PlotId,
        string? Label,
        string PropertyName,
        string PlotName,
        string Type,
        string Status,
        DateTimeOffset OccurredOn = default
    ) :
        BaseIntegrationEvent(
            Guid.NewGuid(),
            SensorId,
            OccurredOn,
            nameof(SensorRegisteredIntegrationEvent),
            new Dictionary<string, Guid> { { "OwnerId", OwnerId }, { "PropertyId", PropertyId }, { "PlotId", PlotId } });
}
