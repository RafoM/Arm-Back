using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransactionCore.Data;

namespace TransactionCore.BackgroundServices
{
    public class PromoExpirationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PromoExpirationService> _logger;

        public PromoExpirationService(IServiceScopeFactory scopeFactory, ILogger<PromoExpirationService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<TransactionCoreDbContext>();

                    var now = DateTime.UtcNow;

                    var expiredPromos = await db.Promos
                        .Where(p => p.ExpirationDate.HasValue
                                    && p.ExpirationDate < now
                                    && p.IsActive)
                        .ToListAsync(stoppingToken);

                    if (expiredPromos.Any())
                    {
                        foreach (var promo in expiredPromos)
                            promo.IsActive = false;

                        await db.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("Deactivated {Count} expired promo(s).", expiredPromos.Count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in PromoExpirationService.");
                }

                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
            }
        }
    }
}
