using TransactionCore.Models.RequestModels;

namespace TransactionCore.Services.Interface
{
    public interface ITronWebhookService
    {
        Task ProcessWebhookAsync(TronWebhookPayload payload);
    }
}
