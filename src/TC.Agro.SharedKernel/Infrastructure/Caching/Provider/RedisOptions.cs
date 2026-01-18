namespace TC.Agro.SharedKernel.Infrastructure.Caching.Provider
{
    public sealed class RedisOptions
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 6379;
        public string Password { get; set; } = "";
        public bool Secure { get; set; } = false;
        public string InstanceName { get; set; } = "tc-agro";

        public string ConnectionString
        {
            get
            {
                var baseConn = $"{Host}:{Port}";

                var parts = new List<string> { baseConn };

                if (!string.IsNullOrWhiteSpace(Password))
                    parts.Add($"password={Password}");

                // StackExchange.Redis options
                parts.Add($"ssl={Secure.ToString().ToLowerInvariant()}");
                parts.Add("abortConnect=False");

                return string.Join(",", parts);
            }
        }
    }
}
