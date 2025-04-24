using TransactionCore.Data.Entity;

namespace TransactionCore.Services.Interface
{
    public interface ISubscriptionUsageService
    {
        Task GrantSubscriptionAsync(Guid userId, Guid packageId, int? bonusDays = null);
    }
}
