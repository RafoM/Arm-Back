using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;

namespace TransactionCore.Services.Interface
{
    public interface IWalletService
    {
        Task DeleteAsync(Guid id);
        Task UpdateAsync(WalletUpdateModel model);
        Task<WalletResponseModel> CreateAsync(WalletRequestModel model);
        Task<WalletResponseModel> GetByIdAsync(Guid id);
        Task<IEnumerable<WalletResponseModel>> GetAllAsync();
        Task UpdateWalletWithTransactionAsync(string walletAddress, decimal transactionAmount, string transactionId);
    }
}
