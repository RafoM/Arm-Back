using MassTransit;
using Microsoft.EntityFrameworkCore;
using TransactionCore.Data;
using TransactionCore.Models.RequestModels;

namespace TransactionCore.BackgroundServices
{
    public class SubscriptionCleanupJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SubscriptionCleanupJob> _logger;

        public SubscriptionCleanupJob(
            IServiceScopeFactory scopeFactory,
            ILogger<SubscriptionCleanupJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<TransactionCoreDbContext>();

                var now = DateTime.UtcNow;

                var expiredUsages = await db.SubscriptionUsages
                    .Include(u => u.UserInfo)
                    .Where(u => u.IsActive && u.ExpiresAt.HasValue && u.ExpiresAt < now)
                    .ToListAsync(stoppingToken);

                foreach (var usage in expiredUsages)
                {
                    usage.IsActive = false;
                    usage.UserInfo.IsSubscribed = false;

                    _logger.LogInformation("Deactivating subscription for UserId: {UserId}", usage.UserInfo.UserId);
                    var publisher = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
                    await publisher.Publish<UpdateUserRole>(new
                    {
                        UserId = usage.UserInfo.UserId,
                        RoleId = 2
                    });
                }

                await db.SaveChangesAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
            }
        }
    }

}
