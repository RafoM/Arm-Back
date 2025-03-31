using System.Globalization;
using TransactionCore.Models.RequestModels;
using TransactionCore.Services.Interface;

namespace TransactionCore.Services.Implementation
{
    public class TronWebhookService : ITronWebhookService
    {
        private readonly IWalletService _walletService;
        
        public TronWebhookService(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task ProcessWebhookAsync(TronWebhookPayload payload)
        {

            //if (!string.Equals(payload.Token, "USDT", StringComparison.OrdinalIgnoreCase))
            //{
            //    throw new ArgumentException("Unsupported token.");
            //}

            if (!decimal.TryParse(payload.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal rawValue))
            {
                throw new ArgumentException("Invalid value format.");
            }
            decimal usdtAmount = rawValue / 1_000_000M;

            await _walletService.UpdateWalletWithTransactionAsync(payload.To, usdtAmount, payload.TransactionId);

        }
    }
}
