using TC.Agro.Contracts.Events.Farm;

namespace TC.Agro.Messaging.Extensions;

/// <summary>
/// Wolverine configuration extensions for Farm Service events
/// Registers farm events with explicit TOPIC routing keys following the pattern:
/// farm.{entity}.{action}
/// 
/// Events published by Farm Service:
/// - farm.property.created (PropertyCreatedIntegrationEvent)
/// - farm.property.updated (PropertyUpdatedIntegrationEvent)
/// - farm.plot.created (PlotCreatedIntegrationEvent)
/// - farm.sensor.registered (SensorRegisteredIntegrationEvent)
/// </summary>
public static class FarmEventsWolverineExtensions
{
    private const string ServiceName = "farm";

    /// <summary>
    /// Configures publishing of Farm service events with explicit routing keys
    /// Ensures Wolverine routes messages with predictable keys for consumer binding
    /// </summary>
    public static void ConfigureFarmEventPublishing(this WolverineOptions opts)
    {
        ArgumentNullException.ThrowIfNull(opts);

        // Register message types with explicit routing key names
        // This ensures Wolverine routes messages with predictable keys for consumer binding

        // ===== PROPERTY EVENTS =====
        opts.RegisterMessageType(
            typeof(EventContext<PropertyCreatedIntegrationEvent>),
            TopicRoutingKeyHelper.GenerateRoutingKey(ServiceName, "property", "created")
        );

        opts.RegisterMessageType(
            typeof(EventContext<PropertyUpdatedIntegrationEvent>),
            TopicRoutingKeyHelper.GenerateRoutingKey(ServiceName, "property", "updated")
        );

        // ===== PLOT EVENTS =====
        opts.RegisterMessageType(
            typeof(EventContext<PlotCreatedIntegrationEvent>),
            TopicRoutingKeyHelper.GenerateRoutingKey(ServiceName, "plot", "created")
        );

        // ===== SENSOR EVENTS =====
        opts.RegisterMessageType(
            typeof(EventContext<SensorRegisteredIntegrationEvent>),
            TopicRoutingKeyHelper.GenerateRoutingKey(ServiceName, "sensor", "registered")
        );
    }

    /// <summary>
    /// Gets the wildcard binding key for consuming all property events
    /// Used by consumers to bind to farm.property.*
    /// </summary>
    public static string GetPropertyEventsWildcardBindingKey()
        => TopicRoutingKeyHelper.GenerateWildcardBindingKey(ServiceName, "property");

    /// <summary>
    /// Gets the wildcard binding key for consuming all plot events
    /// Used by consumers to bind to farm.plot.*
    /// </summary>
    public static string GetPlotEventsWildcardBindingKey()
        => TopicRoutingKeyHelper.GenerateWildcardBindingKey(ServiceName, "plot");

    /// <summary>
    /// Gets the wildcard binding key for consuming all sensor events
    /// Used by consumers to bind to farm.sensor.*
    /// </summary>
    public static string GetSensorEventsWildcardBindingKey()
        => TopicRoutingKeyHelper.GenerateWildcardBindingKey(ServiceName, "sensor");

    /// <summary>
    /// Gets the wildcard binding key for consuming all farm events
    /// Used by consumers to bind to farm.*.*
    /// </summary>
    public static string GetAllFarmEventsWildcardBindingKey()
        => TopicRoutingKeyHelper.GenerateWildcardBindingKey(ServiceName, "*");
}
