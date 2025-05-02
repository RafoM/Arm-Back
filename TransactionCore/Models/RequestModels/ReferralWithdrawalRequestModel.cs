namespace TransactionCore.Models.RequestModels
{
    public class ReferralWithdrawalRequestModel
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Network { get; set; }
        public string ToAddress { get; set; }
    }
}
