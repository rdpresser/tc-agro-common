namespace TC.Agro.Contracts.Events.Farm
{
    /// <summary>
    /// Integration event raised when a plot is created.
    /// Exposes information needed by other services for plot awareness.
    /// </summary>
    /// <remarks>
    /// Inherits from <see cref="BaseIntegrationEvent"/> to provide:
    /// - EventId: unique identifier for this event instance
    /// - AggregateId: identifier of the originating Plot aggregate
    /// - OccurredOn: timestamp when the event occurred
    /// - EventName: event name for logging and auditing
    /// 
    /// Includes RelatedIds with PropertyId for cross-aggregate correlation.
    /// </remarks>
    public sealed record PlotCreatedIntegrationEvent(
        Guid PlotId,
        Guid PropertyId,
        string Name,
        string CropType,
        double AreaHectares,
        DateTimeOffset OccurredOn
    ) : BaseIntegrationEvent(
        Guid.NewGuid(),
        PlotId,
        OccurredOn,
        nameof(PlotCreatedIntegrationEvent),
        new Dictionary<string, Guid> { ["PropertyId"] = PropertyId });
}
