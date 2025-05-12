using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;

namespace TransactionCore.Services.Interface
{
    public interface INetworkService
    {
        Task<IEnumerable<NetworkResponseModel>> GetAllAsync();
        Task<NetworkResponseModel> GetByIdAsync(Guid id);
        Task<NetworkResponseModel> CreateAsync(NetworkRequestModel request);
        Task UpdateAsync(NetworkUpdateModel request);
        Task DeleteAsync(Guid id);
        Task<string> UploadIconAsync(int networkId, IFormFile iconFile);
    }
}
