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
        private readonly IUserInfoService _userInfoService;
        private readonly IPromoService _promoService;
        private readonly ISubscriptionUsageService _subscriptionUsageService;
        private readonly IReferralRoleRewardSerice _referralRoleReward;
        private readonly IRemainderInfoService _remainderInfoService;
        private readonly IReferralService _referralService;
        private readonly ISubscriptionService _subscriptionService;
        public PaymentService(TransactionCoreDbContext dbContext, IUserInfoService userFinance, IPromoService promoService, ISubscriptionUsageService subscriptionUsageService, IReferralRoleRewardSerice referralRoleReward, IRemainderInfoService remainderInfoService, IReferralService referralService, ISubscriptionService subscriptionService)
        {
            _dbContext = dbContext;
            _userInfoService = userFinance;
            _promoService = promoService;
            _subscriptionUsageService = subscriptionUsageService;
            _referralRoleReward = referralRoleReward;
            _remainderInfoService = remainderInfoService;
            _referralService = referralService;
            _subscriptionService = subscriptionService;
        }

        public async Task ApprovePayment(Guid userInfoId, decimal amount, string txHash)
        {
            var paymentDetails = await _dbContext.Payments.FirstOrDefaultAsync(x => x.UserInfoId == userInfoId);
            var userInfo = await _dbContext.UserInfos.FirstOrDefaultAsync(x => x.Id == userInfoId);
            var roleId = await _dbContext.SubscriptionPackages.Where(s => s.Id == paymentDetails.SubscriptionPackageId).Select(x => x.RoleId).FirstOrDefaultAsync();
            var promo = await _dbContext.Promos.FirstOrDefaultAsync(x => x.Id == paymentDetails.PromoId);
            var wallet = await _dbContext.Wallets.FirstOrDefaultAsync(x => x.Id == paymentDetails.WalletId);
            paymentDetails.TxHash = txHash;
            paymentDetails.PayedFee = amount;
            var x = amount - paymentDetails.ExpectedFee;
            var isPayed = false;

            if (x > 0)
            {
                await _remainderInfoService.CreateRemainderInfo(x, wallet.Id, userInfoId);
                userInfo.Balance += x;
                //notifie
            }
            if (x >= 0)
            {
                paymentDetails.PaymentDate = DateTime.UtcNow;
                paymentDetails.Status = PaymentStatusEnum.Paid;
                userInfo.ReferralPurchaseCount += 1;

                await _subscriptionUsageService.GrantSubscriptionAsync(userInfo.UserId, paymentDetails.SubscriptionPackageId, promo.BonusDays);
                var reward = (paymentDetails.ExpectedFee * await _referralRoleReward.GetReferralRewardPercentageAsync(userInfo.ReferrerId, userInfo.ReferralPurchaseCount)) / 100;
                wallet.Status = WalletStatusEnum.Locked;
                paymentDetails.WalletId = null;
                if (userInfo.ReferrerId != null)
                {
                    var referrer = await _dbContext.UserInfos.FirstOrDefaultAsync(x => x.UserId == userInfo.ReferrerId);
                    await _userInfoService.AddReward(referrer.Id, reward, wallet.Id);
                    await _referralService.CreateReferralActivityAsync(userInfo.Id, ReferralActionTypeEnum.Purchase, paymentDetails.Id, reward);
                }
            }
            else if (x < 0)
            {
                userInfo.Balance += amount;
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

            if (userinfo.ReferrerId != null)
            {
                await _referralService.CreateReferralActivityAsync(userinfo.Id, ReferralActionTypeEnum.PaymentAttempt, paymentId);
            }

            return paymentDetails;
        }

        public async Task<PageResultModel<UserPaymentResponseModel>> GetUserPaymentsAsync(Guid userId, int languageId, int pageNumber = 1, int pageSize = 10)
        {
            var paymentsQuery = _dbContext.Payments
                .Include(p => p.Wallet)
                    .ThenInclude(w => w.PaymentMethod)
                        .ThenInclude(pm => pm.Crypto)
                .Include(p => p.Wallet)
                    .ThenInclude(w => w.PaymentMethod)
                        .ThenInclude(pm => pm.Network)
                .Include(p => p.SubscriptionPackage)
                .Where(p => p.UserInfo.UserId == userId)
                .OrderByDescending(p => p.CreatedDate);

            var totalCount = await paymentsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var payments = await paymentsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var responseData = new List<UserPaymentResponseModel>();

            foreach (var payment in payments)
            {
                var (translatedName, _) = await _subscriptionService.GetTranslationAsync(payment.SubscriptionPackage.Id, languageId);

                responseData.Add(new UserPaymentResponseModel
                {
                    Amount = payment.PayedFee ?? 0,
                    Currency = payment.Wallet?.PaymentMethod?.Crypto?.Name ?? string.Empty,
                    Network = payment.Wallet?.PaymentMethod?.Network?.Name ?? string.Empty,
                    Plan = translatedName ?? "Unknown plan",
                    Status = payment.Status.ToString(),
                    CreatedAt = payment.CreatedDate,
                    PaidAt = payment.PaymentDate
                });
            }

            return new PageResultModel<UserPaymentResponseModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = responseData
            };
        }

        private async Task<Guid> CreatePayment(Guid userInfoId, Guid walletId, decimal expectedFee, Guid subscriptionPackageId, Guid? promoId)
        {

            var payment = new Payment
            {
                ExpectedFee = expectedFee,
                WalletId = walletId,
                Status = PaymentStatusEnum.Pending,
                CreatedDate = DateTime.UtcNow,
                UserInfoId = userInfoId,
                SubscriptionPackageId = subscriptionPackageId,
                PromoId = promoId
            };
            await _dbContext.AddAsync(payment);
            await _dbContext.SaveChangesAsync();
            return payment.Id;
        }
    }
}
