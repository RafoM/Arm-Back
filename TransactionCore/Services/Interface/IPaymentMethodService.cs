using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;

namespace TransactionCore.Services.Interface
{
    public interface IPaymentMethodService
    {
        Task<IEnumerable<PaymentMethodResponseModel>> GetAllAsync();
        Task<PaymentMethodResponseModel> GetByIdAsync(Guid id);
        Task<PaymentMethodResponseModel> CreateAsync(PaymentMethodRequestModel request);
        Task UpdateAsync(PaymentMethodUpdateModel request);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<PaymentMethodResponseModel>> GetAllPaymentMethodsByCryptoIdAsync(Guid cryptoId);
    }
}
