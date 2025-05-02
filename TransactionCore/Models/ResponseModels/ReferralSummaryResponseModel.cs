namespace TransactionCore.Models.ResponseModels
{
    public class ReferralSummaryResponseModel
    {
        public int TotalReferrals { get; set; }
        public decimal PotentialEarnings { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal WithdrawalsOrdered { get; set; }
    }
}
