namespace TC.Agro.Contracts.Events.SensorIngested
{
    /// <summary>
    /// Integration event published when sensor data is ingested into the system.
    /// This is the input event that triggers the analytics processing workflow.
    /// </summary>
    public record SensorIngestedIntegrationEvent
    (
        Guid SensorReadingId,
        Guid SensorId,
        DateTimeOffset Time,
        double? Temperature,
        double? Humidity,
        double? SoilMoisture,
        double? Rainfall,
        double? BatteryLevel,
        DateTimeOffset OccurredOn = default
    ) :
        BaseIntegrationEvent(
            Guid.NewGuid(),
            SensorReadingId,
            OccurredOn,
            nameof(SensorIngestedIntegrationEvent),
            new Dictionary<string, Guid> { { "SensorId", SensorId } });

    /// <summary>
    /// Integration event published when a sensor detects high temperature above the configured threshold.
    /// Consumers: Alert Service, Notification Service, Dashboard Service
    /// </summary>
    public record HighTemperatureDetectedIntegrationEvent
    (
        Guid SensorReadingId,
        Guid SensorId,
        DateTimeOffset Time,
        double Temperature,
        double? Humidity,
        double? SoilMoisture,
        double? Rainfall,
        double? BatteryLevel,
        DateTimeOffset OccurredOn = default
    ) :
        BaseIntegrationEvent(
            Guid.NewGuid(),
            SensorReadingId,
            OccurredOn,
            nameof(HighTemperatureDetectedIntegrationEvent),
            new Dictionary<string, Guid> { { "SensorId", SensorId } });

    /// <summary>
    /// Integration event published when low soil moisture is detected below the configured threshold.
    /// This indicates that irrigation may be needed.
    /// Consumers: Irrigation Service, Alert Service, Dashboard Service
    /// </summary>
    public record LowSoilMoistureDetectedIntegrationEvent
    (
        Guid SensorReadingId,
        Guid SensorId,
        DateTimeOffset Time,
        double? Temperature,
        double? Humidity,
        double SoilMoisture,
        double? Rainfall,
        double? BatteryLevel,
        DateTimeOffset OccurredOn = default
    ) :
        BaseIntegrationEvent(
            Guid.NewGuid(),
            SensorReadingId,
            OccurredOn,
            nameof(LowSoilMoistureDetectedIntegrationEvent),
            new Dictionary<string, Guid> { { "SensorId", SensorId } });

    /// <summary>
    /// Integration event published when sensor battery level is low below the configured threshold.
    /// This indicates that sensor maintenance is required.
    /// Consumers: Maintenance Service, Alert Service, Dashboard Service
    /// </summary>
    public record BatteryLowWarningIntegrationEvent
    (
        Guid SensorReadingId,
        Guid SensorId,
        double BatteryLevel,
        double Threshold,
        DateTimeOffset OccurredOn = default
    ) :
        BaseIntegrationEvent(
            Guid.NewGuid(),
            SensorReadingId,
            OccurredOn,
            nameof(BatteryLowWarningIntegrationEvent),
            new Dictionary<string, Guid> { { "SensorId", SensorId } });
}
