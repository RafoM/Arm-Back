using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;

namespace TransactionCore.Services.Interface
{
    public interface IPaymentService
    {
        Task ApprovePayment(Guid userInfoId, decimal amount, string txHash);
        Task<PaymentDetailsResponseModel> GetPaymentDetails(Guid userId, PaymentDetailsRequestModel requestModel);
        Task<PageResultModel<UserPaymentResponseModel>> GetUserPaymentsAsync(Guid userId, int pageNumber = 1, int pageSize = 10);
    }
}
