using Microsoft.EntityFrameworkCore;
using TransactionCore.Common.Enums;
using TransactionCore.Data;
using TransactionCore.Data.Entity;
using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;
using TransactionCore.Services.Interface;

namespace TransactionCore.Services.Implementation
{
    public class WalletService : IWalletService
    {
        private readonly TransactionCoreDbContext _dbContext;

        public WalletService(TransactionCoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<WalletResponseModel>> GetAllAsync()
        {
            var wallets = await _dbContext.Wallets.ToListAsync();
            return wallets.Select(w => new WalletResponseModel
            {
                Id = w.Id,
                Address = w.Address,
                PaymentMethodId = w.PaymentMethodId,
                IsActive = w.IsActive,
                Status = w.Status,
                Balance = w.Balance,
                LastEntry = w.LastEntry ?? 0,
                LastTransactionId = w.LastTransactionId
            });
        }

        public async Task<WalletResponseModel> GetByIdAsync(Guid id)
        {
            var wallet = await _dbContext.Wallets.FindAsync(id);
            if (wallet == null)
                return null;

            return new WalletResponseModel
            {
                Id = wallet.Id,
                Address = wallet.Address,
                PaymentMethodId = wallet.PaymentMethodId,
                IsActive = wallet.IsActive,
                Status = wallet.Status,
                Balance = wallet.Balance,
                LastEntry = wallet.LastEntry ?? 0,
                LastTransactionId = wallet.LastTransactionId
            };
        }

        public async Task<WalletResponseModel> CreateAsync(WalletRequestModel model)
        {
            var wallet = new Wallet
            {
                Id = Guid.NewGuid(),
                Address = model.Address,
                PaymentMethodId = model.PaymentMethodId,
                IsActive = model.IsActive,
                Status = WalletStatusEnum.IsFree,
                Balance = model.Balance,
                LastEntry = 0,
                LastTransactionId = null
            };

            _dbContext.Wallets.Add(wallet);
            await _dbContext.SaveChangesAsync();

            return new WalletResponseModel
            {
                Id = wallet.Id,
                Address = wallet.Address,
                PaymentMethodId = wallet.PaymentMethodId,
                IsActive = wallet.IsActive,
                Status = wallet.Status,
                Balance = wallet.Balance,
                LastEntry = wallet.LastEntry ?? 0,
                LastTransactionId = wallet.LastTransactionId
            };
        }

        public async Task UpdateAsync(WalletUpdateModel model)
        {
            var wallet = await _dbContext.Wallets.FindAsync(model.Id);
            if (wallet == null)
            {
                throw new Exception("Wallet not found.");
            }

            wallet.Address = model.Address;
            wallet.PaymentMethodId = model.PaymentMethodId;
            wallet.IsActive = model.IsActive;
            wallet.Status = model.Status;
            wallet.Balance = model.Balance;

            _dbContext.Wallets.Update(wallet);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var wallet = await _dbContext.Wallets.FindAsync(id);
            if (wallet == null)
            {
                throw new Exception("Wallet not found.");
            }

            _dbContext.Wallets.Remove(wallet);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Finds a wallet by its address and updates its balance and transaction info.
        /// </summary>
        /// <param name="walletAddress">The wallet address.</param>
        /// <param name="transactionAmount">The transaction amount.</param>
        /// <param name="transactionId">The transaction identifier.</param>
        public async Task UpdateWalletWithTransactionAsync(string walletAddress, decimal transactionAmount, string transactionId)
        {
            var wallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.Address == walletAddress);

            if (wallet == null)
            {
                throw new Exception("Wallet not found.");
            }
            
            wallet.Balance += transactionAmount;
            wallet.LastEntry = transactionAmount;
            wallet.LastTransactionId = transactionId;

            wallet.IsActive = true;

            _dbContext.Wallets.Update(wallet);
            await _dbContext.SaveChangesAsync();
        }
    }
}
