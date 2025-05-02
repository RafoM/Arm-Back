namespace TransactionCore.Models.ResponseModels
{
    public class ReferralWithdrawalResponseModel
    {
        public string User { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Network { get; set; }
        public string ToAddress { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? TxHash { get; set; }
    }
}
