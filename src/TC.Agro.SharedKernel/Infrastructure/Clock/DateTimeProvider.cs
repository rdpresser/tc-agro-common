namespace TC.Agro.SharedKernel.Infrastructure.Clock
{
    public sealed class DateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
