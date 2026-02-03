using Serilog.Core;
using Serilog.Events;

namespace TC.Agro.SharedKernel.Api.Extensions
{
    /// <summary>
    /// Serilog enricher that normalizes correlation ID from multiple sources
    /// and ensures it's available as "correlation_id" for Loki filtering.
    /// 
    /// Priority:
    /// 1. LogContext property "correlation_id" (set by TelemetryMiddleware)
    /// 2. LogContext property "CorrelationId" (set by CorrelationMiddleware)
    /// 
    /// This enricher normalizes the naming for consistent Grafana Loki queries:
    /// {correlation_id="abc123"} in Explore
    /// 
    /// Note: Does NOT generate new correlation IDs - that's handled by CorrelationMiddleware.
    /// This enricher only normalizes existing values to snake_case naming.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CorrelationIdNormalizationEnricher : ILogEventEnricher
    {
        private const string CorrelationIdPropertyName = "correlation_id";
        private const string CorrelationIdAlternate = "CorrelationId";

        /// <summary>
        /// Enriches the log event with a normalized correlation_id property.
        /// </summary>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            ArgumentNullException.ThrowIfNull(logEvent);

            // Check if correlation_id already exists (added by TelemetryMiddleware via LogContext)
            if (logEvent.Properties.ContainsKey(CorrelationIdPropertyName))
            {
                // Already present with correct name, nothing to do
                return;
            }

            // Check for alternate naming (CorrelationId from CorrelationMiddleware or WithCorrelationId enricher)
            if (logEvent.Properties.TryGetValue(CorrelationIdAlternate, out var correlationIdProperty))
            {
                // Copy with normalized name (snake_case for Loki filtering)
                var normalizedProperty = propertyFactory.CreateProperty(
                    CorrelationIdPropertyName,
                    correlationIdProperty
                );
                logEvent.AddOrUpdateProperty(normalizedProperty);
            }

            // Note: If no correlation ID exists at all, it means neither CorrelationMiddleware
            // nor TelemetryMiddleware ran (e.g., background job, startup logs).
            // We intentionally don't generate a new one here - let CorrelationMiddleware handle that.
        }
    }
}
