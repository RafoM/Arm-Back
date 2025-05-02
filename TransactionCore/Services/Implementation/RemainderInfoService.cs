using TransactionCore.Data;
using TransactionCore.Data.Entity;
using TransactionCore.Services.Interface;

namespace TransactionCore.Services.Implementation
{
    public class RemainderInfoService : IRemainderInfoService
    {
        private readonly TransactionCoreDbContext _dbContext;

        public RemainderInfoService(TransactionCoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateRemainderInfo(decimal amount, Guid walletId, Guid UserInfoId)
        {
            var remainderInfo = new RemainderInfo
            {
               Amount = amount,
               WalletId = walletId, 
               UserInfoId = UserInfoId  
            };
            await _dbContext.AddAsync(remainderInfo);
            await _dbContext.SaveChangesAsync();
        }
    }
}
