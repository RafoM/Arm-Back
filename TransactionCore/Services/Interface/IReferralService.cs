using TransactionCore.Common.Enums;
using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;

namespace TransactionCore.Services.Interface
{
    public interface IReferralService
    {
        Task CreateReferralPaymentAsync(Guid paymentId, decimal commission);
        Task CreateReferralWithdrawalAsync(Guid userId, ReferralWithdrawalRequestModel request);
        Task<List<ReferralPaymentResponseModel>> GetReferralPaymentsAsync(Guid userId);
        Task<List<ReferralWithdrawalResponseModel>> GetReferralWithdrawalsAsync(Guid userId);
        Task<List<ReferralActivityResponseModel>> GetReferralActivityAsync(Guid userId);
        Task<List<TimedStatResponseModel>> GetPurchasesAsync(Guid userId, int range);
        Task<List<TimedStatResponseModel>> GetRegistrationsAsync(Guid userId, int range);
        Task<ReferralConversionStatsResponseModel> GetReferralConversionStatsAsync(Guid userId);
        Task<ReferralSummaryResponseModel> GetReferralSummaryAsync(Guid userId);
        Task CreateReferralActivityAsync(Guid referredUserInfoId, ReferralActionTypeEnum action, Guid? paymentId = null, decimal? commission = null);
    }
}
