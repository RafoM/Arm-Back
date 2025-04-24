using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;

namespace TransactionCore.Services.Interface
{
    public interface IPaymentService
    {
        Task ApprovePayment(Guid userFinanceId, decimal amount);
        Task<PaymentDetailsResponseModel> GetPaymentDetails(Guid userId, PaymentDetailsRequestModel requestModel);
    }
}
