namespace TC.Agro.Messaging.Routing;

/// <summary>
/// Helper class for generating consistent topic-based routing keys
/// Pattern: {service}.{entity}.{action}
/// Example: identity.user.created
/// </summary>
public static class TopicRoutingKeyHelper
{
    /// <summary>
    /// Generates a routing key for a given event type following the pattern: {service}.{entity}.{action}
    /// </summary>
    /// <param name="service">Service name (e.g., "identity", "farm")</param>
    /// <param name="entity">Entity name (e.g., "user", "property", "plot")</param>
    /// <param name="action">Action name (e.g., "created", "updated", "deactivated")</param>
    /// <returns>Routing key in format: {service}.{entity}.{action}</returns>
    public static string GenerateRoutingKey(string service, string entity, string action)
    {
        if (string.IsNullOrWhiteSpace(service))
            throw new ArgumentException("Service name cannot be empty", nameof(service));
        if (string.IsNullOrWhiteSpace(entity))
            throw new ArgumentException("Entity name cannot be empty", nameof(entity));
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action name cannot be empty", nameof(action));

        return $"{service.ToLowerInvariant()}.{entity.ToLowerInvariant()}.{action.ToLowerInvariant()}";
    }

    /// <summary>
    /// Generates a wildcard binding key for consuming all events of a specific entity
    /// Pattern: {service}.{entity}.*
    /// Example: identity.user.*
    /// </summary>
    public static string GenerateWildcardBindingKey(string service, string entity)
    {
        if (string.IsNullOrWhiteSpace(service))
            throw new ArgumentException("Service name cannot be empty", nameof(service));
        if (string.IsNullOrWhiteSpace(entity))
            throw new ArgumentException("Entity name cannot be empty", nameof(entity));

        return $"{service.ToLowerInvariant()}.{entity.ToLowerInvariant()}.*";
    }

    /// <summary>
    /// Generates a wildcard binding key for consuming all events from a service
    /// Pattern: {service}.*
    /// Example: identity.*
    /// </summary>
    public static string GenerateServiceWildcardBindingKey(string service)
    {
        if (string.IsNullOrWhiteSpace(service))
            throw new ArgumentException("Service name cannot be empty", nameof(service));

        return $"{service.ToLowerInvariant()}.*";
    }
}
