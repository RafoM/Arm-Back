using TransactionCore.Data.Entity;

namespace TransactionCore.Services.Interface
{
    public interface IUserFinanceService
    {
        Task<decimal> GetUserBalanceAsync(Guid userId);
        Task<UserFinance> CreateUserFinanceAsync(Guid userId);
    }
}
