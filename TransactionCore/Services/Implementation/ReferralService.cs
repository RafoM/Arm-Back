using Microsoft.EntityFrameworkCore;
using TransactionCore.Common.Enums;
using TransactionCore.Data;
using TransactionCore.Data.Entity;
using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;
using TransactionCore.Services.Interface;

namespace TransactionCore.Services.Implementation
{
    public class ReferralService : IReferralService
    {
        private readonly TransactionCoreDbContext _dbContext;
        private readonly IReferralRoleRewardSerice _referralRoleReward;
        public ReferralService(TransactionCoreDbContext dbContext, IReferralRoleRewardSerice referralRoleReward)
        {
            _dbContext = dbContext;
            _referralRoleReward = referralRoleReward;
        }


        public async Task<ReferralSummaryResponseModel> GetReferralSummaryAsync(Guid userId)
        {
            var userInfo = await GetCurrentUserInfoAsync(userId);

            var totalReferrals = await _dbContext.UserInfos.CountAsync(u => u.ReferrerId == userInfo.UserId);
            var potentialEarnings = await _dbContext.ReferralPayments
                .Where(r => r.ReferrerUserInfoId == userInfo.Id)
                .SumAsync(r => (decimal?)r.Commission) ?? 0;
            var withdrawalsOrdered = await _dbContext.ReferralWithdrawals
                .Where(w => w.ReferrerUserInfoId == userInfo.Id)
                .SumAsync(w => (decimal?)w.Amount) ?? 0;

            return new ReferralSummaryResponseModel
            {
                TotalReferrals = totalReferrals,
                PotentialEarnings = potentialEarnings,
                CurrentBalance = potentialEarnings - withdrawalsOrdered,
                WithdrawalsOrdered = withdrawalsOrdered
            };
        }

        public async Task<ReferralConversionStatsResponseModel> GetReferralConversionStatsAsync(Guid userId)
        {
            var userInfo = await GetCurrentUserInfoAsync(userId);

            var referredUsers = await _dbContext.UserInfos.Where(u => u.ReferrerId == userInfo.UserId).ToListAsync();
            var referredUserIds = referredUsers.Select(u => u.Id).ToList();

            var purchases = await _dbContext.Payments.CountAsync(p => referredUserIds.Contains(p.UserInfoId) && p.Status == PaymentStatusEnum.Paid);
            var attempts = await _dbContext.Payments.CountAsync(p => referredUserIds.Contains(p.UserInfoId));
            var reward = await _referralRoleReward.GetReferralRewardPercentageAsync(userInfo.Id, userInfo.ReferralPurchaseCount) * 100;
            var currentLevel = new ReferralLevelResponseModel
            {
                Label = (reward / 5).ToString(),
                Percentage = (int)reward
            };

            var nextLevel = new ReferralLevelResponseModel 
            {
                Label = "EarlyAccess",
                Percentage = 40
            };

            return new ReferralConversionStatsResponseModel
            {
                TotalReferralValue = await _dbContext.ReferralPayments
                    .Where(r => r.ReferrerUserInfoId == userInfo.Id)
                    .SumAsync(r => (decimal?)r.Commission) ?? 0,
                Visitors = userInfo.Visits,
                Registered = referredUsers.Count,
                Purchased = purchases,
                CurrentReferralLevel = currentLevel,
                NextLevel = nextLevel
            };
        }

        public async Task<List<TimedStatResponseModel>> GetRegistrationsAsync(Guid userId, int range)
        {
            var userInfo = await GetCurrentUserInfoAsync(userId);
            var start = DateTime.UtcNow.AddDays(-Math.Clamp(range, 1, 365));

            var registrations = _dbContext.UserInfos
                .Where(u => u.ReferrerId == userInfo.UserId && u.CreatedAt >= start)
                .AsEnumerable()
                .GroupBy(u => range <= 1
                    ? u.CreatedAt.ToString("HH:00")
                    : u.CreatedAt.ToString("yyyy-MM-dd"))
                .Select(g => new TimedStatResponseModel
                {
                    Hour = g.Key,
                    Count = g.Count()
                })
                .ToList();

            return registrations;
        }

        public async Task<List<TimedStatResponseModel>> GetPurchasesAsync(Guid userId, int range)
        {
            var userInfo = await GetCurrentUserInfoAsync(userId);
            var start = DateTime.UtcNow.AddDays(-Math.Clamp(range, 1, 365));

            var userIds = await _dbContext.UserInfos
                .Where(u => u.ReferrerId == userInfo.UserId)
                .Select(u => u.Id)
                .ToListAsync();

            var query = _dbContext.Payments
                .Where(p => userIds.Contains(p.UserInfoId) && p.PaymentDate >= start && p.PaymentDate != null)
                .AsEnumerable();

            var grouped = range <= 1
                ? query.GroupBy(p => p.PaymentDate!.Value.ToString("HH:00"))
                : query.GroupBy(p => p.PaymentDate!.Value.ToString("yyyy-MM-dd")); 

            var result = grouped.Select(g => new TimedStatResponseModel
            {
                Hour = g.Key,
                Count = g.Count()
            }).ToList();

            return result;
        }

        public async Task<List<ReferralActivityResponseModel>> GetReferralActivityAsync(Guid userId)
        {
            var userInfo = await GetCurrentUserInfoAsync(userId);

            var activities = await _dbContext.ReferralActivities
                .Include(a => a.Payment).ThenInclude(p => p.SubscriptionPackage)
                .Where(a => a.ReferredUserInfo.ReferrerId == userInfo.UserId)
                .OrderByDescending(a => a.ActionDate)
                .ToListAsync();

            return activities.Select(a => new ReferralActivityResponseModel
            {
                ReferredUser = a.ReferredUserInfo.UserId.ToString(),
                Commission = a.Commission,
                Action = a.Action.ToString() + (a.Payment?.SubscriptionPackage?.Name != null ? $" – {a.Payment.SubscriptionPackage.Name}" : string.Empty),
                ActionDate = a.ActionDate
            }).ToList();
        }

        public async Task<List<ReferralPaymentResponseModel>> GetReferralPaymentsAsync(Guid userId)
        {
            var userInfo = await GetCurrentUserInfoAsync(userId);

            var payments = await _dbContext.ReferralPayments
                .Include(r => r.Payment).ThenInclude(p => p.SubscriptionPackage)
                .Include(r => r.Payment).ThenInclude(p => p.Wallet)
                .Where(r => r.ReferrerUserInfoId == userInfo.Id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return payments.Select(r => new ReferralPaymentResponseModel
            {
                User = r.Payment.UserInfo.DenormalizedEmail,
                Amount = r.Payment.ExpectedFee,
                Currency = r.Payment.Wallet?.PaymentMethod?.Crypto?.Name ?? "USDT",
                Network = r.Payment.Wallet?.PaymentMethod?.Network?.Name ?? "TRON",
                Plan = r.Payment.SubscriptionPackage?.Name ?? "-",
                Status = r.Payment.Status.ToString(),
                CreatedAt = r.Payment.CreatedDate,
                PaidAt = r.Payment.PaymentDate,
                TxHash = r.Payment.Wallet?.LastTransactionId
            }).ToList();
        }

        public async Task<List<ReferralWithdrawalResponseModel>> GetReferralWithdrawalsAsync(Guid userId)
        {
            var userInfo = await GetCurrentUserInfoAsync(userId);

            var withdrawals = await _dbContext.ReferralWithdrawals
                .Where(w => w.ReferrerUserInfoId == userInfo.Id)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

            return withdrawals.Select(w => new ReferralWithdrawalResponseModel
            {
                User = userInfo.DenormalizedEmail,
                Amount = w.Amount,
                Currency = w.Currency,
                Network = w.Network,
                ToAddress = w.ToAddress,
                Status =  w.Status.ToString(),
                CreatedAt = w.CreatedAt,
                ProcessedAt = w.ProcessedAt,
                TxHash = w.TxHash
            }).ToList();
        }

        public async Task CreateReferralWithdrawalAsync(Guid userId, ReferralWithdrawalRequestModel request)
        {
            var userInfo = await GetCurrentUserInfoAsync(userId);

            var withdrawal = new ReferralWithdrawal
            {
                Id = Guid.NewGuid(),
                ReferrerUserInfoId = userInfo.Id,
                Amount = request.Amount,
                Currency = request.Currency,
                Network = request.Network,
                ToAddress = request.ToAddress,
                Status = ReferralWithdrawalStatusEnum.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.ReferralWithdrawals.Add(withdrawal);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<UserInfo> GetCurrentUserInfoAsync(Guid userId)
        {
            return await _dbContext.UserInfos.FirstOrDefaultAsync(u => u.UserId == userId)
                ?? throw new Exception("User info not found.");
        }


        public async Task CreateReferralPaymentAsync(Guid paymentId, decimal commission)
        {
            var payment = await _dbContext.Payments
                .Include(p => p.UserInfo)
                .FirstOrDefaultAsync(p => p.Id == paymentId);

            if (payment == null)
                throw new Exception("Payment not found.");

            if (payment.UserInfo?.ReferrerId == null)
                return; 

            var referralPayment = new ReferralPayment
            {
                PaymentId = paymentId,
                ReferrerUserInfoId = payment.UserInfo.ReferrerId.Value,
                Commission = commission,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.ReferralPayments.Add(referralPayment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateReferralActivityAsync(Guid referredUserInfoId, ReferralActionTypeEnum action, Guid? paymentId = null, decimal? commission = null)
        {
            var referredUser = await _dbContext.UserInfos.FindAsync(referredUserInfoId);
            if (referredUser == null)
                throw new Exception("Referred user not found.");

            var referralActivity = new ReferralActivity
            {
                Id = Guid.NewGuid(),
                ReferredUserInfoId = referredUserInfoId,
                Action = action,
                ActionDate = DateTime.UtcNow,
                PaymentId = paymentId,
                Commission = commission
            };

            _dbContext.ReferralActivities.Add(referralActivity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
