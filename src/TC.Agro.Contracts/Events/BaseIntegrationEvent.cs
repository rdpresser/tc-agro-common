namespace TC.Agro.Contracts.Events
{
    public abstract record BaseIntegrationEvent
    {
        protected BaseIntegrationEvent(
            Guid eventId,
            Guid aggregateId,
            DateTimeOffset? occurredOn,
            string eventName,
            IDictionary<string, Guid>? relatedIds = null)
        {
            EventId = eventId;
            AggregateId = aggregateId;
            OccurredOn = occurredOn is null || !occurredOn.HasValue || occurredOn.Value == default
                ? DateTimeOffset.UtcNow
                : occurredOn.Value;
            EventName = eventName;
            RelatedIds = relatedIds;
        }

        public Guid EventId { get; init; }

        public Guid AggregateId { get; init; }

        public DateTimeOffset OccurredOn { get; init; }

        public string EventName { get; init; }

        public IDictionary<string, Guid>? RelatedIds { get; init; }
    }
}
