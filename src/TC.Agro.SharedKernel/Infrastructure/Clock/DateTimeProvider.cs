namespace TC.Agro.SharedKernel.Infrastructure.Clock
{
    public sealed class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
