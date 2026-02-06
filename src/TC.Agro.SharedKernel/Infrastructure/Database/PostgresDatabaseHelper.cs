namespace TC.Agro.SharedKernel.Infrastructure.Database
{
    public static class PostgresDatabaseHelper
    {
        public async static Task EnsureDatabaseExists(DbConnectionFactory dbConnectionFactory)
        {
            var maintenanceConnStr = dbConnectionFactory.MaintenanceConnectionString;
            var targetConnStr = dbConnectionFactory.ConnectionString;

            // Debug: Log the connection strings to understand what's happening
            ////Console.WriteLine($"Maintenance Connection String: {maintenanceConnStr}");
            ////Console.WriteLine($"Target Connection String: {targetConnStr}");

            // Parse database name and user from connection string
            var builder = new NpgsqlConnectionStringBuilder(targetConnStr);
            var databaseName = builder.Database;
            var user = builder.Username;

            try
            {
                using var conn = new NpgsqlConnection(maintenanceConnStr);
                await conn.OpenAsync().ConfigureAwait(false);

                // Check if database exists
                if (databaseName == null)
                    throw new InvalidOperationException("Database name could not be determined from connection string.");

                using var cmd = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = @dbname", conn);
                cmd.Parameters.AddWithValue("dbname", databaseName);
                var exists = await cmd.ExecuteScalarAsync().ConfigureAwait(false) != null;

                if (!exists)
                {
                    using var createCmd = new NpgsqlCommand($"CREATE DATABASE \"{databaseName}\" OWNER \"{user}\" ENCODING 'UTF8';", conn);
                    await createCmd.ExecuteNonQueryAsync();
                }
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                throw new InvalidOperationException(
                    $"Não foi possível conectar ao servidor PostgreSQL. " +
                    $"Verifique se o host está correto e acessível. " +
                    $"Connection string: {maintenanceConnStr.Replace(builder.Password ?? "", "***")} " +
                    $"Erro original: {ex.Message}", ex);
            }
        }
    }
}
