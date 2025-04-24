using System.Globalization;
using System.Numerics;
using System.Text.Json;
using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;
using TransactionCore.Services.Interface;

namespace TransactionCore.Services.Implementation
{
    public class TronWebhookService : ITronWebhookService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWalletService _walletService;
        private readonly IConfiguration _config;

        public TronWebhookService(IHttpClientFactory httpClientFactory, IWalletService walletService, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _walletService = walletService;
            _config = config;
        }

        public async Task<TransactionCheckResultModel> CheckTransactionOnChainAsync(string transactionId)
        {
            var client = _httpClientFactory.CreateClient();
            var quickNodeUrl = _config["QuickNode:TronHttpUrl"] ?? throw new Exception("QuickNode URL not configured");

            var rpcPayload = new
            {
                jsonrpc = "2.0",
                method = "eth_getTransactionReceipt",
                @params = new[] { transactionId },
                id = 1
            };

            var response = await client.PostAsJsonAsync(quickNodeUrl, rpcPayload);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to fetch transaction from chain.");
            }

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var result = doc.RootElement.GetProperty("result");

            if (result.ValueKind == JsonValueKind.Null)
            {
                return new TransactionCheckResultModel
                {
                    TransactionId = transactionId,
                    Found = false
                };
            }

            var logs = result.GetProperty("logs");
            foreach (var log in logs.EnumerateArray())
            {
                var topics = log.GetProperty("topics");
                var toHex = topics[2].GetString();
                var toAddress = "41" + toHex[^40..]; 

                var data = log.GetProperty("data").GetString();
                var rawValue = Convert.ToDecimal(BigInteger.Parse(data.Substring(2), NumberStyles.HexNumber));
                decimal amount = rawValue / 1_000_000M; 

                bool belongsToUser = await _walletService.WalletExistsAsync(toAddress);

                return new TransactionCheckResultModel
                {
                    TransactionId = transactionId,
                    Found = true,
                    Confirmed = true,
                    To = toAddress,
                    Amount = amount,
                    Token = log.GetProperty("address").GetString(),
                    BelongsToUser = belongsToUser
                };
            }

            return new TransactionCheckResultModel
            {
                TransactionId = transactionId,
                Found = true,
                Confirmed = false
            };
        }

        public async Task ProcessWebhookAsync(TronWebhookPayload payload)
        {
            if (string.IsNullOrWhiteSpace(payload.Token) ||
                string.IsNullOrWhiteSpace(payload.To) ||
                string.IsNullOrWhiteSpace(payload.Value) ||
                string.IsNullOrWhiteSpace(payload.TransactionId))
            {
                throw new ArgumentException("Payload is missing required fields.");
            }

            if (!decimal.TryParse(payload.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal rawValue))
            {
                throw new ArgumentException("Invalid token value format.");
            }

            decimal factor = (decimal)Math.Pow(10, payload.Decimals);
            decimal amount = rawValue / factor;

            await _walletService.UpdateWalletWithTokenTransactionAsync(
                toAddress: payload.To,
                amount: amount,
                txId: payload.TransactionId
            );
        }
    }
}
