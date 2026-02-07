namespace TC.Agro.Contracts.Events.Analytics
{
    /// <summary>
    /// Integration event published when sensor data is ingested into the system.
    /// This is the input event that triggers the analytics processing workflow.
    /// </summary>
    public record SensorIngestedIntegrationEvent
    (
        Guid EventId,
        Guid AggregateId,
        DateTimeOffset OccurredOn,
        string EventName,
        IDictionary<string, Guid>? RelatedIds,
        string SensorId,
        Guid PlotId,
        DateTime Time,
        double? Temperature,
        double? Humidity,
        double? SoilMoisture,
        double? Rainfall,
        double? BatteryLevel
    ) : BaseIntegrationEvent(EventId, AggregateId, OccurredOn, EventName, RelatedIds);

    /// <summary>
    /// Integration event published when a sensor detects high temperature above the configured threshold.
    /// Consumers: Alert Service, Notification Service, Dashboard Service
    /// </summary>
    public record HighTemperatureDetectedIntegrationEvent
    (
        Guid EventId,
        Guid AggregateId,
        DateTimeOffset OccurredOn,
        string EventName,
        IDictionary<string, Guid>? RelatedIds,
        string SensorId,
        Guid PlotId,
        DateTime Time,
        double Temperature,
        double? Humidity,
        double? SoilMoisture,
        double? Rainfall,
        double? BatteryLevel
    ) : BaseIntegrationEvent(EventId, AggregateId, OccurredOn, EventName, RelatedIds);

    /// <summary>
    /// Integration event published when low soil moisture is detected below the configured threshold.
    /// This indicates that irrigation may be needed.
    /// Consumers: Irrigation Service, Alert Service, Dashboard Service
    /// </summary>
    public record LowSoilMoistureDetectedIntegrationEvent
    (
        Guid EventId,
        Guid AggregateId,
        DateTimeOffset OccurredOn,
        string EventName,
        IDictionary<string, Guid>? RelatedIds,
        string SensorId,
        Guid PlotId,
        DateTime Time,
        double? Temperature,
        double? Humidity,
        double SoilMoisture,
        double? Rainfall,
        double? BatteryLevel
    ) : BaseIntegrationEvent(EventId, AggregateId, OccurredOn, EventName, RelatedIds);

    /// <summary>
    /// Integration event published when sensor battery level is low below the configured threshold.
    /// This indicates that sensor maintenance is required.
    /// Consumers: Maintenance Service, Alert Service, Dashboard Service
    /// </summary>
    public record BatteryLowWarningIntegrationEvent
    (
        Guid EventId,
        Guid AggregateId,
        DateTimeOffset OccurredOn,
        string EventName,
        IDictionary<string, Guid>? RelatedIds,
        string SensorId,
        Guid PlotId,
        double BatteryLevel,
        double Threshold
    ) : BaseIntegrationEvent(EventId, AggregateId, OccurredOn, EventName, RelatedIds);
}
