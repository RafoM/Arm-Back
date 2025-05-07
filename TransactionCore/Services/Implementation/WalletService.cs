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
        private readonly IPaymentService _paymentService;
        private readonly ILogger<WalletService> _logger;

        public WalletService(TransactionCoreDbContext dbContext, IPaymentService paymentService, ILogger<WalletService> logger)
        {
            _dbContext = dbContext;
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task<bool> WalletExistsAsync(string walletAddress)
        {
            return await _dbContext.Wallets.AnyAsync(w => w.Address == walletAddress);
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
        public async Task UpdateWalletWithTokenTransactionAsync(string toAddress, decimal amount, string txId)
        {
            var wallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.Address == toAddress);
            var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.WalletId == wallet.Id);
            if (wallet == null)
            {
                throw new Exception("Wallet not found.");
            }

            wallet.Balance += amount;
            wallet.LastEntry = amount;
            wallet.LastTransactionId = txId;

            wallet.IsActive = true;

            _dbContext.Wallets.Update(wallet);
            await _dbContext.SaveChangesAsync();

            await _paymentService.ApprovePayment(payment.UserInfoId, amount, txId);
        }

        public async Task UnlockAndDetachWalletAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            var payment = await _dbContext.Payments
                .Include(p => p.Wallet)
                .FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);

            if (payment == null || payment.Wallet == null)
            {
                _logger.LogWarning("Payment {PaymentId} or associated wallet not found.", paymentId);
                return;
            }

            payment.Wallet.Status = WalletStatusEnum.IsFree;
            payment.WalletId = null;

            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Wallet {WalletId} freed and detached from payment {PaymentId}", payment.Wallet.Id, payment.Id);
        }
    }
}
