using Microsoft.EntityFrameworkCore;
using TransactionCore.Common.Enums;
using TransactionCore.Data;
using TransactionCore.Data.Entity;
using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;
using TransactionCore.Services.Interface;

namespace TransactionCore.Services.Implementation
{
    public class PaymentService : IPaymentService
    {
        private readonly TransactionCoreDbContext _dbContext;
        private readonly IUserFinanceService _userFinance;
        public PaymentService(TransactionCoreDbContext dbContext, IUserFinanceService userFinance)
        {
            _dbContext = dbContext;
            _userFinance = userFinance;
        }

        public async Task ApprovePayment(Guid userFinanceId, decimal amount)
        {
            var paymentDetails = await _dbContext.Payments.FirstOrDefaultAsync(x => x.UserFinanceId == userFinanceId);
            var userFinance = await _dbContext.UserFinances.FirstOrDefaultAsync(x => x.Id == userFinanceId);
            
            var x = paymentDetails.ExpectedFee - amount;
                
            if (x > 0) 
            { 
                userFinance.AmountWalletId = paymentDetails.WalletId; 
                userFinance.Balance += x;
            }
            if (x == 0) 
            {
                var wallet = await _dbContext.Wallets.FirstOrDefaultAsync(x => x.Id == paymentDetails.WalletId);
                wallet.Status = WalletStatusEnum.IsFree;
                paymentDetails.WalletId = null;

            }
            if (x >= 0)
            {
                paymentDetails.PaymentDate = DateTime.UtcNow;
                paymentDetails.Status = PaymentStatusEnum.Paid;
                //add subscription access for user
            }
            else if (x < 0)
            {
                userFinance.Balance += amount;
                paymentDetails.CreatedDate = DateTime.UtcNow;
                //notifie
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PaymentDetailsResponseModel> GetPaymentDetails(Guid userId, PaymentDetailsRequestModel requestModel)
        {
            var userFinance = await _dbContext.UserFinances.FirstOrDefaultAsync(x => x.UserId == userId);
            if(userFinance == null)  userFinance = await _userFinance.CreateUserFinanceAsync(userId);
            if (userFinance.ExpectedPaymentId != null)
            {
                var payment = await _dbContext.Payments.FirstOrDefaultAsync(x => x.Id == userFinance.ExpectedPaymentId);
                var actualWallet = await _dbContext.Wallets.FirstOrDefaultAsync(x => x.Id == payment.WalletId);
                payment.Status = PaymentStatusEnum.NotActual;
                payment.WalletId = null;
                actualWallet.Status = WalletStatusEnum.IsFree;
                await _dbContext.SaveChangesAsync();
            }
            var subscription = await _dbContext.SubscriptionPackages.FirstOrDefaultAsync(x => x.Id == requestModel.SubscriptionPackageId);
            if (subscription == null)
                throw new Exception("Subscription package not found.");

            var paymentMethod = await _dbContext.PaymentMethods.FirstOrDefaultAsync(x => x.NetworkId == requestModel.NetworkId && 
                                                                                         x.CryptoId == requestModel.CryptoId);

            if (paymentMethod == null)
                throw new Exception("Active payment method not found for the given network and crypto.");

            var wallet = await _dbContext.Wallets.FirstOrDefaultAsync(x => x.Status == WalletStatusEnum.IsFree && 
                                                                           x.PaymentMethodId == paymentMethod.Id);
            if (wallet == null)
                throw new Exception("No free wallet available for the selected payment method.");

            var subPrice = subscription.Price * requestModel.Duration;
            var price = subPrice - (subPrice * (decimal)subscription.Discount) / 100;

            var paymentId = await CreatePayment(userFinance.Id, wallet.Id, price);

            var paymentDetails = new PaymentDetailsResponseModel
            {
                Price = price,
                TransactionFee = paymentMethod.TransactionFee,
                WaletAddress = wallet.Address,
                Balance = userFinance.Balance,
                ExpectedFee = (price - userFinance.Balance <= 0)?  0 : price - userFinance.Balance
            };

            
            wallet.Status = WalletStatusEnum.Pending;
            userFinance.ExpectedPaymentId = paymentId;
            await _dbContext.SaveChangesAsync();

            return paymentDetails;
        }

        private async Task<Guid> CreatePayment(Guid userFinanceId, Guid walletId, decimal expectedFee)
        {

            var payment = new Payment
            {
                ExpectedFee = expectedFee,
                WalletId = walletId,
                Status = PaymentStatusEnum.Pending,
                CreatedDate = DateTime.UtcNow,
                UserFinanceId = userFinanceId
            };
            await _dbContext.AddAsync(payment);
            await _dbContext.SaveChangesAsync();
            return payment.Id;
        }
    }
}
