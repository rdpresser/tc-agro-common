namespace TC.Agro.SharedKernel.Infrastructure.MessageBroker
{
    public sealed class RabbitMqOptions
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public int ManagementPort { get; set; } = 15672;
        public string VirtualHost { get; set; } = "/";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string Exchange { get; set; } = "unknown.events";
        public bool AutoProvision { get; set; } = true;
        public bool Durable { get; set; } = true;
        public bool UseQuorumQueues { get; set; } = false;
        public bool AutoPurgeOnStartup { get; set; } = true;
        public string ConnectionString => BuildAmqpUri();

        private string BuildAmqpUri()
        {
            var vhost = string.IsNullOrWhiteSpace(VirtualHost) ? "/" : VirtualHost.Trim();

            // garante que começa com "/"
            if (!vhost.StartsWith('/'))
                vhost = "/" + vhost;

            // "/" (default vhost) precisa ser url-encoded -> "%2F"
            if (vhost == "/")
                vhost = "/%2F";

            return $"amqp://{UserName}:{Password}@{Host}:{Port}{vhost}";
        }
    }
}
