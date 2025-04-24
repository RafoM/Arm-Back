using TransactionCore.Data.Entity;
using TransactionCore.Data;
using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;
using TransactionCore.Services.Interface;

namespace TransactionCore.Services.Implementation
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly TransactionCoreDbContext _dbContext;

        public SubscriptionService(TransactionCoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task UpdateUserSubscription(Guid userFinanceId)
        {

        }

        public async Task<IEnumerable<SubscriptionResponseModel>> GetAllAsync(int? languageId)
        {
            if (languageId == null) languageId = 1;
            var packages = await _dbContext.SubscriptionPackages.Where(x => x.LanguageId == languageId).ToListAsync();
            return packages.Select(p => new SubscriptionResponseModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Duration = p.Duration,
                Price = p.Price,
                Discount = p.Discount,
                Currency = p.Currency
            });
        }

        public async Task<SubscriptionResponseModel> GetByIdAsync(int id)
        {
            var p = await _dbContext.SubscriptionPackages.FindAsync(id);
            if (p == null) return null;

            return new SubscriptionResponseModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Duration = p.Duration,
                Price = p.Price,
                Discount = p.Discount,
                Currency = p.Currency
            };
        }

        public async Task<SubscriptionResponseModel> CreateAsync(SubscriptionRequestModel request)
        {
            var package = new SubscriptionPackage
            {
                LanguageId = request.LanguageId,
                Name = request.Name,
                Description = request.Description,
                Duration = request.Duration,
                Price = request.Price,
                Discount = request.Discount,
                Currency = request.Currency,
                RoleId = request.RoleId,
            };

            _dbContext.SubscriptionPackages.Add(package);
            await _dbContext.SaveChangesAsync();

            return new SubscriptionResponseModel
            {
                Id = package.Id,
                Name = package.Name,
                Description = package.Description,
                Duration = package.Duration,
                Price = package.Price,
                Discount = package.Discount,
                Currency = package.Currency
            };
        }

        public async Task UpdateAsync(SubscriptionUpdateModel request)
        {
            var existing = await _dbContext.SubscriptionPackages.FindAsync(request.Id);
            if (existing == null)
                throw new Exception("Subscription package not found");

            existing.Name = request.Name;
            existing.Description = request.Description;
            existing.Duration = request.Duration;
            existing.Price = request.Price;
            existing.Discount = request.Discount;
            existing.Currency = request.Currency;
            existing.RoleId = request.RoleId;

            _dbContext.SubscriptionPackages.Update(existing);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _dbContext.SubscriptionPackages.FindAsync(id);
            if (existing == null)
                throw new Exception("Subscription package not found");

            _dbContext.SubscriptionPackages.Remove(existing);
            await _dbContext.SaveChangesAsync();
        }
    }
}
