namespace TC.Agro.Contracts.Events.Farm
{
    /// <summary>
    /// Integration event raised when a sensor's connectivity status changes.
    /// 
    /// This event is published by the Sensor Ingest Service when detecting changes in:
    /// Online, Warning, or Offline connectivity states based on data reception patterns.
    /// 
    /// Differs from operational status (Active/Inactive/Maintenance/Faulty):
    /// - Operational Status: Managed by human decisions (set via Farm API)
    /// - Connectivity Status: Detected automatically by ingestion pipeline
    /// 
    /// Used by:
    /// - Farm Service: updates sensor connectivity display in UI
    /// - Analytics Worker: may trigger connectivity alerts
    /// - Dashboard Service: shows real-time sensor availability
    /// </summary>
    /// <remarks>
    /// Version: 1
    /// Routing Key: sensor-ingest.sensor.connectivity-status
    /// </remarks>
    public record SensorConnectivityStatusChangedIntegrationEvent(
        Guid EventId,
        Guid AggregateId,                    // SensorId
        DateTimeOffset OccurredOn,
        
        // Payload
        Guid SensorId,
        Guid PlotId,
        string PreviousConnectivity,         // "Online", "Warning", "Offline"
        string NewConnectivity,              // "Online", "Warning", "Offline"
        
        // Optional
        string EventName = "SensorConnectivityStatusChanged",
        int? SignalStrength = null,          // -80 dBm (WiFi/Signal strength)
        int? BatteryLevel = null,            // 85 (percentage)
        DateTimeOffset? LastDataAt = null,   // When last reading was received
        string? DisconnectReason = null,     // "No data for 10 minutes", "Connection timeout", etc.
        IDictionary<string, Guid>? RelatedIds = null
    ) : BaseIntegrationEvent(EventId, AggregateId, OccurredOn, EventName, RelatedIds);
}
