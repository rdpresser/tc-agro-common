namespace TC.Agro.SharedKernel.Infrastructure.Database
{
    /// <summary>
    /// Default database schema names for PostgreSQL.
    /// Services can override or extend these defaults.
    /// </summary>
    public static class DefaultSchemas
    {
        /// <summary>
        /// Default PostgreSQL schema for application tables.
        /// </summary>
        public const string Default = "public";

        /// <summary>
        /// Schema for Wolverine message persistence (outbox/inbox).
        /// </summary>
        public const string Wolverine = "wolverine";
    }
}
