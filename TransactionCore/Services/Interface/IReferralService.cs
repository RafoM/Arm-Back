using TransactionCore.Common.Enums;
using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;

namespace TransactionCore.Services.Interface
{
    public interface IReferralService
    {
        Task CreateReferralWithdrawalAsync(Guid userId, ReferralWithdrawalRequestModel request);
        Task<List<TimedStatResponseModel>> GetPurchasesAsync(Guid userId, int range);
        Task<List<TimedStatResponseModel>> GetRegistrationsAsync(Guid userId, int range);
        Task<ReferralConversionStatsResponseModel> GetReferralConversionStatsAsync(Guid userId, string role);
        Task<ReferralSummaryResponseModel> GetReferralSummaryAsync(Guid userId);
        Task<PageResultModel<ReferralActivityResponseModel>> GetReferralActivityAsync(
        Guid userId, int languageId, int pageNumber = 1, int pageSize = 10);
        Task<PageResultModel<ReferralWithdrawalResponseModel>> GetReferralWithdrawalsAsync(Guid userId, int pageNumber = 1, int pageSize = 10);
        Task<PageResultModel<ReferralPaymentResponseModel>> GetReferralPaymentsAsync(
            Guid userId, int languageId, int pageNumber = 1, int pageSize = 10);
        Task CreateReferralActivityAsync(Guid referredUserInfoId, ReferralActionTypeEnum action, Guid? paymentId = null, decimal? commission = null);
    }
}
