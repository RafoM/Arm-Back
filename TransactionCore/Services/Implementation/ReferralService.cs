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
        private readonly ISubscriptionService _subscriptionService;
        public ReferralService(TransactionCoreDbContext dbContext, IReferralRoleRewardSerice referralRoleReward, ISubscriptionService subscriptionService)
        {
            _dbContext = dbContext;
            _referralRoleReward = referralRoleReward;
            _subscriptionService = subscriptionService;
        }


        public async Task<ReferralSummaryResponseModel> GetReferralSummaryAsync(Guid userId)
        {
            var userInfo = await GetCurrentUserInfoAsync(userId);

            var totalReferrals = await _dbContext.UserInfos
                .CountAsync(u => u.ReferrerId == userInfo.UserId);

            var potentialEarnings = await _dbContext.ReferralActivities
                .Where(a => a.ReferredUserInfo.ReferrerId == userInfo.UserId &&
                            a.Action == ReferralActionTypeEnum.Purchase &&
                            a.Commission.HasValue)
                .SumAsync(a => (decimal?)a.Commission) ?? 0;

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


        public async Task<ReferralConversionStatsResponseModel> GetReferralConversionStatsAsync(Guid userId, string role)
        {
            var userInfo = await GetCurrentUserInfoAsync(userId);

            var referredUsers = await _dbContext.UserInfos
                .Where(u => u.ReferrerId == userInfo.UserId)
                .ToListAsync();
            var referredUserIds = referredUsers.Select(u => u.Id).ToList();

            var activities = await _dbContext.ReferralActivities
                .Where(a => referredUserIds.Contains(a.ReferredUserInfoId))
                .ToListAsync();

            var purchases = activities
                .Count(a => a.Action == ReferralActionTypeEnum.Purchase);

            var attempts = activities
                .Count(a => a.Action == ReferralActionTypeEnum.PaymentAttempt);

            var totalValue = activities
                .Where(a => a.Action == ReferralActionTypeEnum.Purchase && a.Commission.HasValue)
                .Sum(a => a.Commission.Value);

            var reward = await _referralRoleReward.GetReferralRewardPercentageAsync(userInfo.Id, userInfo.ReferralPurchaseCount) * 100;
            var currentLevel = new ReferralLevelResponseModel
            {
                Label = userInfo.IsSubscribed ? role : (reward / 5).ToString(),
                Percentage = (int)reward
            };

            ReferralLevelResponseModel nextLevel = null;

            if (!userInfo.IsSubscribed)
            {
                nextLevel = new ReferralLevelResponseModel
                {
                    Label = currentLevel.Label == "5" ? "EarlyAccess" : (reward / 5 + 1).ToString(),
                    Percentage = 40
                };
            }

            return new ReferralConversionStatsResponseModel
            {
                TotalReferralValue = totalValue,
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

        public async Task<PageResultModel<ReferralActivityResponseModel>> GetReferralActivityAsync(
      Guid userId, int languageId, int pageNumber = 1, int pageSize = 10)
        {
            var userInfo = await GetCurrentUserInfoAsync(userId);

            var query = _dbContext.ReferralActivities
                .Include(a => a.ReferredUserInfo)
                .Include(a => a.Payment).ThenInclude(p => p.SubscriptionPackage)
                .Where(a => a.ReferredUserInfo.ReferrerId == userInfo.UserId)
                .OrderByDescending(a => a.ActionDate);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = new List<ReferralActivityResponseModel>();

            foreach (var activity in items)
            {
                string? planName = null;

                if (activity.Payment?.SubscriptionPackage != null)
                {
                    var (translatedName, _) = await _subscriptionService
                        .GetTranslationAsync(activity.Payment.SubscriptionPackage.Id, languageId);
                    planName = translatedName;
                }

                data.Add(new ReferralActivityResponseModel
                {
                    ReferredUser = activity.ReferredUserInfo.UserId.ToString(),
                    Commission = activity.Commission,
                    Action = activity.Action.ToString() + (planName != null ? $" – {planName}" : string.Empty),
                    ActionDate = activity.ActionDate
                });
            }

            return new PageResultModel<ReferralActivityResponseModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalCount = totalCount,
                Data = data
            };
        }


        public async Task<PageResultModel<ReferralPaymentResponseModel>> GetReferralPaymentsAsync(
          Guid userId, int languageId, int pageNumber = 1, int pageSize = 10)
        {
            var userInfo = await GetCurrentUserInfoAsync(userId);

            var query = _dbContext.ReferralActivities
                .Include(a => a.Payment).ThenInclude(p => p.SubscriptionPackage)
                .Include(a => a.Payment).ThenInclude(p => p.Wallet).ThenInclude(w => w.PaymentMethod).ThenInclude(pm => pm.Crypto)
                .Include(a => a.Payment).ThenInclude(p => p.Wallet).ThenInclude(w => w.PaymentMethod).ThenInclude(pm => pm.Network)
                .Include(a => a.Payment).ThenInclude(p => p.UserInfo)
                .Where(a => a.Action == ReferralActionTypeEnum.Purchase &&
                            a.Payment != null &&
                            a.Payment.UserInfo.ReferrerId == userInfo.UserId)
                .OrderByDescending(a => a.ActionDate);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = new List<ReferralPaymentResponseModel>();

            foreach (var r in items)
            {
                string? planName = "-";
                if (r.Payment?.SubscriptionPackage != null)
                {
                    var (translatedName, _) = await _subscriptionService.GetTranslationAsync(
                        r.Payment.SubscriptionPackage.Id,
                        languageId
                    );
                    planName = translatedName ?? "-";
                }

                data.Add(new ReferralPaymentResponseModel
                {
                    Email = r.Payment.UserInfo.DenormalizedEmail,
                    Amount = r.Payment.ExpectedFee,
                    Currency = r.Payment.Wallet?.PaymentMethod?.Crypto?.Name,
                    Network = r.Payment.Wallet?.PaymentMethod?.Network?.Name,
                    Plan = planName,
                    Status = r.Payment.Status.ToString(),
                    CreatedAt = r.Payment.CreatedDate,
                    PaidAt = r.Payment.PaymentDate
                });
            }

            return new PageResultModel<ReferralPaymentResponseModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalCount = totalCount,
                Data = data
            };
        }


        public async Task<PageResultModel<ReferralWithdrawalResponseModel>> GetReferralWithdrawalsAsync(
    Guid userId, int pageNumber = 1, int pageSize = 10)
        {
            var userInfo = await GetCurrentUserInfoAsync(userId);

            var query = _dbContext.ReferralWithdrawals
                .Where(w => w.ReferrerUserInfoId == userInfo.Id)
                .OrderByDescending(w => w.CreatedAt);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = data.Select(w => new ReferralWithdrawalResponseModel
            {
                User = userInfo.DenormalizedEmail,
                Amount = w.Amount,
                Currency = w.Currency,
                Network = w.Network,
                ToAddress = w.ToAddress,
                Status = w.Status.ToString(),
                CreatedAt = w.CreatedAt,
                ProcessedAt = w.ProcessedAt,
                TxHash = w.TxHash
            });

            return new PageResultModel<ReferralWithdrawalResponseModel>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalCount = totalCount,
                Data = mapped.ToList()
            };
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
