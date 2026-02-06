using Serilog.Core;
using Serilog.Events;

namespace TC.Agro.SharedKernel.Api.Extensions
{
    /// <summary>
    /// Serilog enricher that converts UTC timestamps to local time based on a specified timezone.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UtcToLocalTimeEnricher : ILogEventEnricher
    {
        private readonly TimeZoneInfo _timeZone;

        /// <summary>
        /// Initializes a new instance of the <see cref="UtcToLocalTimeEnricher"/> class.
        /// </summary>
        /// <param name="timeZone">The timezone to convert to.</param>
        public UtcToLocalTimeEnricher(TimeZoneInfo timeZone)
        {
            _timeZone = timeZone ?? throw new ArgumentNullException(nameof(timeZone));
        }

        /// <summary>
        /// Enriches the log event with a LocalTimestamp property.
        /// </summary>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            ArgumentNullException.ThrowIfNull(logEvent);

            try
            {
                // Convert UTC to local time using the specified timezone
                var localTimestamp = TimeZoneInfo.ConvertTimeFromUtc(logEvent.Timestamp.UtcDateTime, _timeZone);

                // Add a custom property for the local timestamp
                var localTimestampProperty = propertyFactory.CreateProperty(
                    "LocalTimestamp",
                    localTimestamp.ToString("yyyy-MM-dd HH:mm:ss")
                );

                logEvent.AddOrUpdateProperty(localTimestampProperty);
            }
            catch (TimeZoneNotFoundException)
            {
                throw new InvalidOperationException($"The timezone '{_timeZone.Id}' is not recognized on this system.");
            }
            catch (InvalidTimeZoneException)
            {
                throw new InvalidOperationException($"The timezone '{_timeZone.Id}' is invalid or corrupted.");
            }
        }
    }
}
