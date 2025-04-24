using TransactionCore.Data.Entity;

namespace TransactionCore.Services.Interface
{
    public interface IUserInfoService
    {
        Task<decimal> GetUserBalanceAsync(Guid userId);
        Task UpdateUserRoleAsync(Guid userId, int roleId);
        Task<UserInfo> CreateUserinfoAsync(Guid userId, string? promoCode = null, Guid? referrerId = null);
    }
}
