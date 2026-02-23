namespace TC.Agro.Contracts.Events.Farm
{
    /// <summary>
    /// Integration event raised when a sensor's operational status changes.
    /// 
    /// This event is published by the Farm Service when a sensor status transitions between:
    /// Active, Inactive, Maintenance, or Faulty.
    /// 
    /// Used by:
    /// - Sensor Ingest Service: synchronizes operational status via SensorAggregate
    /// - Analytics Worker: may trigger alert rules based on status changes
    /// - Dashboard Service: updates sensor status displays in real-time
    /// </summary>
    /// <remarks>
    /// Differs from SensorDeactivatedIntegrationEvent:
    /// - Status Change: Reversible transitions (Active ↔ Maintenance)
    /// - Deactivation: Permanent soft-delete (IsActive = false)
    /// 
    /// Version: 1 (supports future 2.0 with StatusChangeType and PolicyRuleId)
    /// </remarks>
    public record SensorOperationalStatusChangedIntegrationEvent(
        Guid SensorId,
        Guid OwnerId,
        Guid PlotId,
        Guid PropertyId,
        string? Label,
        string PropertyName,
        string PlotName,
        string Status,
        string? Reason,
        DateTimeOffset OccurredOn = default
    ) :
        BaseIntegrationEvent(
            Guid.NewGuid(),
            SensorId,
            OccurredOn,
            nameof(SensorOperationalStatusChangedIntegrationEvent),
            new Dictionary<string, Guid> { { "OwnerId", OwnerId }, { "PlotId", PlotId }, { "PropertyId", PropertyId } });
}
