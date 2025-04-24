using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;

namespace TransactionCore.Services.Interface
{
    public interface IWalletService
    {
        Task DeleteAsync(Guid id);
        Task UpdateAsync(WalletUpdateModel model);
        Task<WalletResponseModel> CreateAsync(WalletRequestModel model);
        Task<bool> WalletExistsAsync(string walletAddress);
        Task<WalletResponseModel> GetByIdAsync(Guid id);
        Task<IEnumerable<WalletResponseModel>> GetAllAsync();
        Task UpdateWalletWithTokenTransactionAsync(string toAddress, decimal amount, string txId);
        Task UnlockAndDetachWalletAsync(Guid paymentId, CancellationToken cancellationToken = default);
    }
}
