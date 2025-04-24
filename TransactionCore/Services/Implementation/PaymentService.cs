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
        private readonly IUserInfoService _userFinance;
        private readonly IPromoService _promoService;
        private readonly SubscriptionUsageService _subscriptionUsageService;
        public PaymentService(TransactionCoreDbContext dbContext, IUserInfoService userFinance, IPromoService promoService, SubscriptionUsageService subscriptionUsageService)
        {
            _dbContext = dbContext;
            _userFinance = userFinance;
            _promoService = promoService;
            _subscriptionUsageService = subscriptionUsageService;
        }

        public async Task ApprovePayment(Guid userFinanceId, decimal amount)
        {
            var paymentDetails = await _dbContext.Payments.FirstOrDefaultAsync(x => x.UserFinanceId == userFinanceId);
            var userFinance = await _dbContext.UserInfos.FirstOrDefaultAsync(x => x.Id == userFinanceId);
            var roleId = await _dbContext.SubscriptionPackages.Where(s => s.Id == paymentDetails.SubscriptionPackageId).Select(x => x.RoleId).FirstOrDefaultAsync();
            var promo = await _dbContext.Promos.FirstOrDefaultAsync(x => x.Id == paymentDetails.PromoId);
            var x = paymentDetails.ExpectedFee - amount;
            var isPayed = false;    
            
            if (x > 0) 
            { 
                userFinance.AmountWalletId = paymentDetails.WalletId; 
                userFinance.Balance += x;
                //notifie
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
                await _subscriptionUsageService.GrantSubscriptionAsync(userFinance.UserId, paymentDetails.SubscriptionPackageId, promo.BonusDays);
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
            if (string.IsNullOrEmpty(requestModel.PromoCode))                  
            {
               await _promoService.ApplyPromoToUserAsync(userId, requestModel.PromoCode);
            }
            var userinfo = await _dbContext.UserInfos.FirstOrDefaultAsync(x => x.UserId == userId);
            if(userinfo == null)  userinfo = await _userFinance.CreateUserinfoAsync(userId);
            if (userinfo.ExpectedPaymentId != null)
            {
                var payment = await _dbContext.Payments.FirstOrDefaultAsync(x => x.Id == userinfo.ExpectedPaymentId);
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

            var promo = await _promoService.GetUserPromoAsync(userId);

            var price = subscription.Price - (subscription.Price * (decimal)subscription.Discount) / 100;
            if (promo != null)
            {
                price = price - ((price * (decimal)promo.Promo.DiscountPercent) / 100);
            }
            var paymentId = await CreatePayment(userinfo.Id, wallet.Id, price, requestModel.SubscriptionPackageId, promo.Id);

            var paymentDetails = new PaymentDetailsResponseModel
            {
                Price = price,
                TransactionFee = paymentMethod.TransactionFee,
                WaletAddress = wallet.Address,
                Balance = userinfo.Balance,
                ExpectedFee = price,
            };

            
            wallet.Status = WalletStatusEnum.Pending;
            userinfo.ExpectedPaymentId = paymentId;
            await _dbContext.SaveChangesAsync();

            return paymentDetails;
        }

        private async Task<Guid> CreatePayment(Guid userFinanceId, Guid walletId, decimal expectedFee, Guid subscriptionPackageId, Guid? promoId)
        {

            var payment = new Payment
            {
                ExpectedFee = expectedFee,
                WalletId = walletId,
                Status = PaymentStatusEnum.Pending,
                CreatedDate = DateTime.UtcNow,
                UserFinanceId = userFinanceId,
                SubscriptionPackageId = subscriptionPackageId,
                PromoId = promoId   
            };
            await _dbContext.AddAsync(payment);
            await _dbContext.SaveChangesAsync();
            return payment.Id;
        }
    }
}
