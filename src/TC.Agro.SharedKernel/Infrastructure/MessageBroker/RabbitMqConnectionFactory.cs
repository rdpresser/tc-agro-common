namespace TC.Agro.SharedKernel.Infrastructure.MessageBroker
{
    public sealed class RabbitMqConnectionFactory
    {
        private readonly RabbitMqOptions _options;

        public RabbitMqConnectionFactory(IOptions<RabbitMqOptions> options)
        {
            _options = options.Value;
        }

        public RabbitMqOptions Options => _options;

        public string ConnectionString => _options.ConnectionString;
    }
}
