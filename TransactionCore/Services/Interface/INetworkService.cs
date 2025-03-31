using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;

namespace TransactionCore.Services.Interface
{
    public interface INetworkService
    {
        Task<IEnumerable<NetworkUpdateModel>> GetAllAsync();
        Task<NetworkUpdateModel> GetByIdAsync(Guid id);
        Task<NetworkUpdateModel> CreateAsync(NetworkRequestModel request);
        Task UpdateAsync(NetworkResponseModel request);
        Task DeleteAsync(Guid id);
    }
}
