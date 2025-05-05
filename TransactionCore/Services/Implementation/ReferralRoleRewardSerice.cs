using Arbito.Shared.Contracts.Identity;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using TransactionCore.Data;
using TransactionCore.Services.Interface;

namespace TransactionCore.Services.Implementation
{
    public class ReferralRoleRewardSerice : IReferralRoleRewardSerice
    {
        private readonly TransactionCoreDbContext _dbContext;
        private readonly IRequestClient<IGetUserRoleRequest> _userRoleClient;

        public ReferralRoleRewardSerice(TransactionCoreDbContext dbContext, IRequestClient<IGetUserRoleRequest> userRoleClient)
        {
            _dbContext = dbContext;
            _userRoleClient = userRoleClient;
        }

        public async Task<decimal> GetReferralRewardPercentageAsync(Guid? referrerId, int referralCount)
        {
            var userRole = "";
            if (referrerId != null)
            {
                var response = await _userRoleClient.GetResponse<IGetUserRoleResponse>(new
                {
                    UserId = referrerId
                });

                userRole = response.Message.Role;
            }
            var roleConfig = await _dbContext.ReferralRoleRewardConfigs
                .FirstOrDefaultAsync(r => r.Role == userRole);

            if (roleConfig != null)
            {
                if (roleConfig.FixedPercentage.HasValue)
                    return roleConfig.FixedPercentage.Value;
            }

            var level = CalculateReferralRewardPercentage(referralCount);

            return level;
        }

        private decimal CalculateReferralRewardPercentage(int referredPurchaseCount)
        {

            return referredPurchaseCount switch
            {
                1 => 0.05m,
                2 => 0.10m,
                3 => 0.15m,
                4 => 0.20m,
                >= 5 => 0.25m,
                _ => 0.00m
            };
        }
    }
}
