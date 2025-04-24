using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;

namespace TransactionCore.Services.Interface
{
    public interface ITronWebhookService
    {
        Task ProcessWebhookAsync(TronWebhookPayload payload);
        Task<TransactionCheckResultModel> CheckTransactionOnChainAsync(string transactionId);
    }
}
