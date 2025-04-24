using Microsoft.EntityFrameworkCore;
using TransactionCore.Data;
using TransactionCore.Data.Entity;
using TransactionCore.Services.Interface;

namespace TransactionCore.Services.Implementation
{
    public class PromoService : IPromoService
    {
        private readonly TransactionCoreDbContext _dbContext;

        public PromoService(TransactionCoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ApplyPromoToUserAsync(Guid userId, string code)
        {
            var promo = await _dbContext.Promos
                .FirstOrDefaultAsync(p => p.Code == code && p.IsActive &&
                    (p.ExpirationDate == null || p.ExpirationDate > DateTime.UtcNow));

            if (promo == null) return false;

            if (promo.MaxUsageCount.HasValue && promo.UsedCount >= promo.MaxUsageCount.Value)
                return false;

            var usage = new PromoUsage
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PromoId = promo.Id,
                UsedAt = DateTime.UtcNow
            };

            promo.UsedCount++;

            _dbContext.PromoUsages.Add(usage);
            _dbContext.Promos.Update(promo);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<PromoUsage> GetUserPromoAsync(Guid userId)
        {
            return await _dbContext.PromoUsages
                .Include(pu => pu.Promo)
                .Where(pu => pu.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Promo>> GetAllAsync()
        {
            return await _dbContext.Promos.ToListAsync();
        }

        public async Task<Promo?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Promos.FindAsync(id);
        }

        public async Task<Promo> CreateAsync(Promo promo)
        {
            promo.Id = Guid.NewGuid();
            promo.IsActive = true;
            promo.UsedCount = 0;

            _dbContext.Promos.Add(promo);
            await _dbContext.SaveChangesAsync();

            return promo;
        }

        public async Task<bool> UpdateAsync(Guid id, Promo updatedPromo)
        {
            if (id != updatedPromo.Id) return false;

            var promo = await _dbContext.Promos.FindAsync(id);
            if (promo == null) return false;

            promo.Code = updatedPromo.Code;
            promo.DiscountPercent = updatedPromo.DiscountPercent;
            promo.BonusDays = updatedPromo.BonusDays;
            promo.ExpirationDate = updatedPromo.ExpirationDate;
            promo.IsActive = updatedPromo.IsActive;
            promo.MaxUsageCount = updatedPromo.MaxUsageCount;

            _dbContext.Promos.Update(promo);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var promo = await _dbContext.Promos.FindAsync(id);
            if (promo == null) return false;

            _dbContext.Promos.Remove(promo);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
