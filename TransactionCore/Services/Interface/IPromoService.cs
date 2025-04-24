using TransactionCore.Data.Entity;

namespace TransactionCore.Services.Interface
{
    public interface IPromoService
    {
        Task<List<Promo>> GetAllAsync();
        Task<Promo?> GetByIdAsync(Guid id);
        Task<Promo> CreateAsync(Promo promo);
        Task<bool> UpdateAsync(Guid id, Promo updatedPromo);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ApplyPromoToUserAsync(Guid userId, string code);
        Task<PromoUsage> GetUserPromoAsync(Guid userId);
    }
}
