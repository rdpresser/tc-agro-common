namespace TC.Agro.Messaging.Extensions;

/// <summary>
/// Wolverine configuration extensions for generic TOPIC-based fanout consumption
/// </summary>
public static class TopicFanoutWolverineExtensions
{
    public static void ConfigureIdentityUserEventsConsumption(
        this WolverineOptions opts,
        string exchangeName = "identity.events-exchange",
        string queueName = "farm-identity-user-events-queue",
        bool isDurable = true)
    {
        ConfigureTopicFanoutConsumption(
            opts,
            sourceService: "identity",
            entity: "user",
            exchangeName: exchangeName,
            queueName: queueName,
            isDurable: isDurable);
    }

    public static void ConfigureFarmSensorEventsConsumption(
        this WolverineOptions opts,
        string exchangeName = "farm.events-exchange",
        string queueName = "sensor-ingest-farm-sensor-events-queue",
        bool isDurable = true)
    {
        ConfigureTopicFanoutConsumption(
            opts,
            sourceService: "farm",
            entity: "sensor",
            exchangeName: exchangeName,
            queueName: queueName,
            isDurable: isDurable);
    }

    /// <summary>
    /// Configures consumption with TOPIC exchange type and wildcard routing key
    /// Binds to {sourceService}.{entity}.*
    /// </summary>
    public static void ConfigureTopicFanoutConsumption(
        this WolverineOptions opts,
        string sourceService,
        string entity,
        string exchangeName,
        string queueName,
        bool isDurable = true)
    {
        ArgumentNullException.ThrowIfNull(opts);
        if (string.IsNullOrWhiteSpace(sourceService))
            throw new ArgumentException("Source service name cannot be empty", nameof(sourceService));
        if (string.IsNullOrWhiteSpace(entity))
            throw new ArgumentException("Entity name cannot be empty", nameof(entity));
        if (string.IsNullOrWhiteSpace(exchangeName))
            throw new ArgumentException("Exchange name cannot be empty", nameof(exchangeName));
        if (string.IsNullOrWhiteSpace(queueName))
            throw new ArgumentException("Queue name cannot be empty", nameof(queueName));

        var bindingKey = TopicRoutingKeyHelper.GenerateWildcardBindingKey(sourceService, entity);

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
