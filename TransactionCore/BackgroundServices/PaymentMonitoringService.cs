using Microsoft.EntityFrameworkCore;
using TransactionCore.Common.Enums;
using TransactionCore.Data;
using TransactionCore.Services.Interface;

namespace TransactionCore.BackgroundServices
{
    public class PaymentMonitoringService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PaymentMonitoringService> _logger;

        public PaymentMonitoringService(
            IServiceScopeFactory scopeFactory,
            ILogger<PaymentMonitoringService> logger)
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

                    var expiredPayments = await db.Payments
                        .Include(p => p.Wallet)
                        .Where(p =>
                            p.Status == PaymentStatusEnum.Pending &&
                            p.WalletId != null &&
                            p.CreatedDate.AddHours(1) < now &&
                            p.Wallet.Status != WalletStatusEnum.Locked)
                        .ToListAsync(stoppingToken);

                    foreach (var payment in expiredPayments)
                    {
                        payment.Wallet.Status = WalletStatusEnum.Locked;
                        _logger.LogInformation("Locked wallet {WalletId} for expired payment {PaymentId}", payment.Wallet.Id, payment.Id);

                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

                                using var delayedScope = _scopeFactory.CreateScope();
                                var walletService = delayedScope.ServiceProvider.GetRequiredService<IWalletService>();

                                await walletService.UnlockAndDetachWalletAsync(payment.Id, stoppingToken);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error during delayed wallet release for payment {PaymentId}", payment.Id);
                            }
                        }, stoppingToken);
                    }

                    await db.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in PaymentMonitoringService");
                }

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken); 
            }
        }
    }

}
