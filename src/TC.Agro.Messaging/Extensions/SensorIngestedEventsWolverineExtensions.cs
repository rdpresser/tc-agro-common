using TC.Agro.Contracts.Events.SensorIngested;

namespace TC.Agro.Messaging.Extensions;

/// <summary>
/// Wolverine configuration extensions for Identity Service events
/// Registers user events with explicit TOPIC routing keys following the pattern:
/// identity.user.{action}
/// </summary>
public static class SensorIngestedEventsWolverineExtensions
{
    private const string ServiceName = "sensor-ingest";
    private const string EntityName = "sensor";

    /// <summary>
    /// Configures publishing of Sensor Ingest service sensor events with explicit routing keys
    /// </summary>
    public static void ConfigureSensorIngestedEventPublishing(this WolverineOptions opts)
    {
        ArgumentNullException.ThrowIfNull(opts);

        // Register message types with explicit routing key names
        // This ensures Wolverine routes messages with predictable keys for consumer binding

        opts.RegisterMessageType(
            typeof(EventContext<SensorIngestedIntegrationEvent>),
            TopicRoutingKeyHelper.GenerateRoutingKey(ServiceName, EntityName, "ingested")
        );
    }

    /// <summary>
    /// Gets the wildcard binding key for consuming all sensor events
    /// Used by consumers to bind to sensor-ingest.sensor.*
    /// </summary>
    public static string GetSensorEventsWildcardBindingKey()
        => TopicRoutingKeyHelper.GenerateWildcardBindingKey(ServiceName, EntityName);
}
