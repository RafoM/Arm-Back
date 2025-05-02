namespace TransactionCore.Models.ResponseModels
{
    public class ReferralPaymentResponseModel
    {
        public string User { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Network { get; set; }
        public string Plan { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? TxHash { get; set; }
    }
}
