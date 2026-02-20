namespace TC.Agro.Contracts.Events.Farm
{
    /// <summary>
    /// Integration event raised when a sensor is permanently deactivated (soft-deleted).
    /// 
    /// This event marks a sensor as logically deleted from the system, distinguishing it
    /// from status changes which are reversible transitions.
    /// 
    /// Triggers cascading deactivation across services:
    /// - Sensor Ingest Service: marks SensorSnapshot.IsActive = false
    /// - Analytics Worker: stops processing alert rules for this sensor
    /// - Dashboard Service: removes sensor from active monitoring displays
    /// 
    /// Differences:
    /// - Status Change (SensorOperationalStatusChangedIntegrationEvent): Reversible (Active ↔ Maintenance)
    /// - Deactivation: Permanent soft-delete within the system (IsActive = false)
    /// </summary>
    /// <remarks>
    /// Version: 1
    /// Routing Key: farm.sensor.deactivated
    /// IsActive flag on aggregates will be set to false upon receiving this event.
    /// </remarks>
    public record SensorDeactivatedIntegrationEvent(
        Guid EventId,
        Guid AggregateId,                    // SensorId
        DateTimeOffset OccurredOn,
        
        // Payload
        Guid SensorId,
        Guid PlotId,
        Guid PropertyId,
        string Reason,                       // "Moved to another farm", "End of lifecycle", "Replacement"
        Guid DeactivatedByUserId,
        
        // Optional
        string EventName = "SensorDeactivated",
        IDictionary<string, Guid>? RelatedIds = null
    ) : BaseIntegrationEvent(EventId, AggregateId, OccurredOn, EventName, RelatedIds);
}
