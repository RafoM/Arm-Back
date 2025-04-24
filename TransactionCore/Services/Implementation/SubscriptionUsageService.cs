using MassTransit;
using Microsoft.EntityFrameworkCore;
using TransactionCore.Data;
using TransactionCore.Data.Entity;
using TransactionCore.Models.RequestModels;
using TransactionCore.Services.Interface;

namespace TransactionCore.Services.Implementation
{
    public class SubscriptionUsageService : ISubscriptionUsageService
    {
        private readonly TransactionCoreDbContext _dbContext;
        private readonly IPublishEndpoint _publishEndpoint;

        public SubscriptionUsageService(TransactionCoreDbContext dbContext, IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _publishEndpoint = publishEndpoint;
        }

        public async Task GrantSubscriptionAsync(Guid userId, Guid packageId, int? bonusDays = null)
        {
            var userInfo = await _dbContext.UserInfos.FirstOrDefaultAsync(u => u.UserId == userId);
            if (userInfo == null)
                throw new Exception("UserInfo not found");

            var package = await _dbContext.SubscriptionPackages.FindAsync(packageId);
            if (package == null)
                throw new Exception("Package not found");

            var usage = new SubscriptionUsage
            {
                Id = Guid.NewGuid(),
                UserInfoId = userInfo.Id,
                SubscriptionPackageId = package.Id,
                ActivatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(package.Duration + (bonusDays ?? 0)),
                PromoBonusDays = bonusDays,
                IsActive = true
            };

            userInfo.IsSubscribed = true;
            await _publishEndpoint.Publish<UpdateUserRole>(new
            {
                UserId = usage.UserInfo.UserId,
                RoleId = package.RoleId
            });
            _dbContext.SubscriptionUsages.Add(usage);
            await _dbContext.SaveChangesAsync();
        }
    }
}
