namespace TC.Agro.SharedKernel.Infrastructure.Authentication
{
    public interface ITokenProvider
    {
        string Create(UserTokenProvider user);
    }
}
