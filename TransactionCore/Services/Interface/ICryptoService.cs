using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;

namespace TransactionCore.Services.Interface
{
    public interface ICryptoService
    {
        Task<IEnumerable<CryptoResponseModel>> GetAllAsync();
        Task<CryptoResponseModel> GetByIdAsync(Guid id);
        Task<CryptoResponseModel> CreateAsync(CryptoRequestModel request);
        Task UpdateAsync(CryptoUpdateModel request);
        Task DeleteAsync(Guid id);
        Task<string> UploadIconAsync(int cryptoId, IFormFile iconFile);
    }
}
