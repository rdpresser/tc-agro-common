using Wolverine;
using TC.Agro.Contracts.Events.Identity;
using TC.Agro.Messaging.Routing;

namespace TC.Agro.Messaging.Extensions;

/// <summary>
/// Wolverine configuration extensions for Identity Service events
/// Registers user events with explicit TOPIC routing keys following the pattern:
/// identity.user.{action}
/// </summary>
public static class IdentityEventsWolverineExtensions
{
    private const string ServiceName = "identity";
    private const string EntityName = "user";

    /// <summary>
    /// Configures publishing of Identity service user events with explicit routing keys
    /// </summary>
    public static void ConfigureIdentityEventPublishing(this WolverineOptions opts)
    {
        ArgumentNullException.ThrowIfNull(opts);

        // Register message types with explicit routing key names
        // This ensures Wolverine routes messages with predictable keys for consumer binding

        opts.RegisterMessageType(
            typeof(EventContext<UserCreatedIntegrationEvent>),
            TopicRoutingKeyHelper.GenerateRoutingKey(ServiceName, EntityName, "created")
        );

        opts.RegisterMessageType(
            typeof(EventContext<UserUpdatedIntegrationEvent>),
            TopicRoutingKeyHelper.GenerateRoutingKey(ServiceName, EntityName, "updated")
        );

        opts.RegisterMessageType(
            typeof(EventContext<UserDeactivatedIntegrationEvent>),
            TopicRoutingKeyHelper.GenerateRoutingKey(ServiceName, EntityName, "deactivated")
        );
    }

    /// <summary>
    /// Gets the wildcard binding key for consuming all user events
    /// Used by consumers to bind to identity.user.*
    /// </summary>
    public static string GetUserEventsWildcardBindingKey()
        => TopicRoutingKeyHelper.GenerateWildcardBindingKey(ServiceName, EntityName);
}
