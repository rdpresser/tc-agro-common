namespace TC.Agro.Messaging.Extensions;

/// <summary>
/// Wolverine configuration extensions for Farm Service event consumption
/// Handles subscribing to inter-service events with TOPIC-based wildcard binding
/// </summary>
public static class FarmServiceWolverineExtensions
{
    /// <summary>
    /// Configures Farm Service to consume Identity Service user events
    /// Binds to identity.user.* with TOPIC exchange type
    /// </summary>
    public static void ConfigureIdentityUserEventsConsumption(
        this WolverineOptions opts,
        string exchangeName = "identity.events-exchange",
        string queueName = "farm-identity-user-events-queue")
    {
        ArgumentNullException.ThrowIfNull(opts);
        if (string.IsNullOrWhiteSpace(exchangeName))
            throw new ArgumentException("Exchange name cannot be empty", nameof(exchangeName));
        if (string.IsNullOrWhiteSpace(queueName))
            throw new ArgumentException("Queue name cannot be empty", nameof(queueName));

        var bindingKey = TopicRoutingKeyHelper.GenerateWildcardBindingKey("identity", "user");

        opts.ListenToRabbitQueue(queueName, configure =>
        {
            // Bind with TOPIC exchange and wildcard key
            configure.BindExchange(
                exchangeName: exchangeName,
                bindingKey: bindingKey,
                arguments: new Dictionary<string, object>
                {
                    ["x-exchange-type"] = "topic"  // Enforce TOPIC exchange type
                });
        }).UseDurableInbox();
    }

    /// <summary>
    /// Configures generic inter-service event consumption
    /// Binds to {service}.{entity}.* with TOPIC exchange type
    /// </summary>
    public static void ConfigureTopicConsumption(
        this WolverineOptions opts,
        string sourcService,
        string entity,
        string exchangeName,
        string queueName,
        bool isDurable = true)
    {
        ArgumentNullException.ThrowIfNull(opts);
        if (string.IsNullOrWhiteSpace(sourcService))
            throw new ArgumentException("Source service name cannot be empty", nameof(sourcService));
        if (string.IsNullOrWhiteSpace(entity))
            throw new ArgumentException("Entity name cannot be empty", nameof(entity));
        if (string.IsNullOrWhiteSpace(exchangeName))
            throw new ArgumentException("Exchange name cannot be empty", nameof(exchangeName));
        if (string.IsNullOrWhiteSpace(queueName))
            throw new ArgumentException("Queue name cannot be empty", nameof(queueName));

        var bindingKey = TopicRoutingKeyHelper.GenerateWildcardBindingKey(sourcService, entity);

        var queueConfig = opts.ListenToRabbitQueue(queueName, configure =>
        {
            configure.BindExchange(
                exchangeName: exchangeName,
                bindingKey: bindingKey,
                arguments: new Dictionary<string, object>
                {
                    ["x-exchange-type"] = "topic"
                });
        });

        if (isDurable)
        {
            queueConfig.UseDurableInbox();
        }
    }
}
