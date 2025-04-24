namespace TransactionCore.Services.Interface
{
    public interface IAuthService
    {
        Task<string> AuthorizeMicroserviceAsync();
    }
}
