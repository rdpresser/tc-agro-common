namespace TC.Agro.SharedKernel.Domain.Events
{
    public record BaseDomainEvent(Guid AggregateId, DateTimeOffset OccurredOn);
}
