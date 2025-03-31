using Microsoft.EntityFrameworkCore;
using TransactionCore.Data;
using TransactionCore.Data.Entity;
using TransactionCore.Services.Interface;

namespace TransactionCore.Services.Implementation
{
    public class UserFinanceService : IUserFinanceService
    {
        private readonly TransactionCoreDbContext _dbContext;

        public UserFinanceService(TransactionCoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<decimal> GetUserBalanceAsync(Guid userId)
        {
            var userFinance = await _dbContext.UserFinances.FirstOrDefaultAsync(x => x.UserId == userId);
            if (userFinance == null) { await CreateUserFinanceAsync(userId); return 0; }
            return userFinance.Balance;
        }

        public async Task<UserFinance> CreateUserFinanceAsync(Guid userId)
        {
            var userFinance = new UserFinance 
            {
                UserId = userId,
                Balance = 0
            };
            await _dbContext.AddAsync(userFinance);
            await _dbContext.SaveChangesAsync();

            return userFinance;
        }
    }
}
