namespace TC.Agro.SharedKernel.Infrastructure.Middleware
{
    public interface ICorrelationIdGenerator
    {
        string CorrelationId { get; }
        void SetCorrelationId(string correlationId);
    }
}
