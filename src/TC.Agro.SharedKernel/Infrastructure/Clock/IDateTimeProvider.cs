namespace TC.Agro.SharedKernel.Infrastructure.Clock
{
    /// <summary>
    /// Inteface for providing the current date and time.
    /// Using interface is possible to mock and test the code more easily.
    /// </summary>
    public interface IDateTimeProvider
    {
        DateTimeOffset UtcNow { get; }
    }
}
