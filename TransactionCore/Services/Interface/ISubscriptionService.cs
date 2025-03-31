using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;

namespace TransactionCore.Services.Interface
{
    public interface ISubscriptionService
    {
        Task<IEnumerable<SubscriptionResponseModel>> GetAllAsync(int? languageId);
        Task<SubscriptionResponseModel> GetByIdAsync(int id);
        Task<SubscriptionResponseModel> CreateAsync(SubscriptionRequestModel request);
        Task UpdateAsync(SubscriptionUpdateModel request);
        Task DeleteAsync(int id);
    }
}
