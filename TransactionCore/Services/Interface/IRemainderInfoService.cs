namespace TransactionCore.Services.Interface
{
    public interface IRemainderInfoService
    {
        Task CreateRemainderInfo(decimal amount, Guid walletId, Guid UserInfoId);
    }
}
