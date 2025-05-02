using TransactionCore.Data.Entity;

namespace TransactionCore.Services.Interface
{
    public interface IUserInfoService
    {
        
        //Task UpdateUserRoleAsync(Guid userId, int roleId);
        Task<UserInfo> CreateUserinfoAsync(Guid userId, string email, string? promoCode = null, Guid? referrerId = null);
        Task AddReward(Guid userInfoId, decimal reward, Guid walletId);
    }
}
