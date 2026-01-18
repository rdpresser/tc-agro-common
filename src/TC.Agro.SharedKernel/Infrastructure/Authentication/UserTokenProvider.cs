namespace TC.Agro.SharedKernel.Infrastructure.Authentication
{
    public sealed record UserTokenProvider(Guid Id, string Name, string Email, string Username, string Role);
}
