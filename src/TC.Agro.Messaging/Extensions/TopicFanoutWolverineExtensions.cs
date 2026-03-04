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
        bool isDurable = true,
        bool enableWiretapQueue = false,
        string wiretapQueuePrefix = "TEST-")
    {
        ConfigureTopicFanoutConsumption(
            opts,
            sourceService: "identity",
            entity: "user",
            exchangeName: exchangeName,
            queueName: queueName,
            isDurable: isDurable,
            enableWiretapQueue: enableWiretapQueue,
            wiretapQueuePrefix: wiretapQueuePrefix);
    }

    public static void ConfigureFarmSensorEventsConsumption(
        this WolverineOptions opts,
        string exchangeName = "farm.events-exchange",
        string queueName = "sensor-ingest-farm-sensor-events-queue",
        bool isDurable = true,
        bool enableWiretapQueue = false,
        string wiretapQueuePrefix = "TEST-")
    {
        ConfigureTopicFanoutConsumption(
            opts,
            sourceService: "farm",
            entity: "sensor",
            exchangeName: exchangeName,
            queueName: queueName,
            isDurable: isDurable,
            enableWiretapQueue: enableWiretapQueue,
            wiretapQueuePrefix: wiretapQueuePrefix);
    }

    public static void ConfigureSensorIngestSensorEventsConsumption(
        this WolverineOptions opts,
        string exchangeName = "sensor-ingest.events-exchange",
        string queueName = "analytics-sensor-ingest-events-queue",
        bool isDurable = true,
        bool enableWiretapQueue = false,
        string wiretapQueuePrefix = "TEST-")
    {
        ConfigureTopicFanoutConsumption(
            opts,
            sourceService: "sensor-ingest",
            entity: "sensor",
            exchangeName: exchangeName,
            queueName: queueName,
            isDurable: isDurable,
            enableWiretapQueue: enableWiretapQueue,
            wiretapQueuePrefix: wiretapQueuePrefix);
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
        bool isDurable = true,
        bool enableWiretapQueue = false,
        string wiretapQueuePrefix = "TEST-")
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
        if (enableWiretapQueue && string.IsNullOrWhiteSpace(wiretapQueuePrefix))
            throw new ArgumentException("Wiretap queue prefix cannot be empty when wiretap queue is enabled", nameof(wiretapQueuePrefix));

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

        if (!enableWiretapQueue)
        {
            return;
        }

        var wiretapQueueName = $"{wiretapQueuePrefix}{queueName}";
        if (string.Equals(wiretapQueueName, queueName, StringComparison.Ordinal))
            throw new ArgumentException("Wiretap queue name cannot be the same as the main queue name", nameof(wiretapQueuePrefix));

        var rabbitMq = opts.ConfigureRabbitMq();

        rabbitMq.DeclareQueue(wiretapQueueName, queue =>
        {
            queue.IsDurable = isDurable;
            queue.BindExchange(
                exchangeName,
                bindingKey,
                new Dictionary<string, object>
                {
                    ["x-exchange-type"] = "topic"
                });
        });
    }
}
