namespace TC.Agro.SharedKernel.Infrastructure.MessageBroker
{
    public sealed class RabbitMqHelper
    {
        private const string RabbitMqSectionName = "Messaging:RabbitMQ";

        // --------------------------------------------------
        // RabbitMQ configuration loaded from appsettings and environment variables
        // --------------------------------------------------
        public RabbitMqOptions RabbitMqSettings { get; }

        public RabbitMqHelper(IConfiguration configuration)
        {
            // Bind section "RabbitMq" → RabbitMqOptions
            RabbitMqSettings = configuration.GetSection(RabbitMqSectionName).Get<RabbitMqOptions>()
                               ?? new RabbitMqOptions();
        }

        // --------------------------------------------------
        // Static convenience method to get configured RabbitMQ options
        // --------------------------------------------------
        public static RabbitMqOptions Build(IConfiguration configuration) =>
            new RabbitMqHelper(configuration).RabbitMqSettings;
    }
}
