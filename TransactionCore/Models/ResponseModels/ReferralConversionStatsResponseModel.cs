namespace TransactionCore.Models.ResponseModels
{
    public class ReferralConversionStatsResponseModel
    {
        public decimal TotalReferralValue { get; set; }
        public int Visitors { get; set; }
        public int Registered { get; set; }
        public int Purchased { get; set; }
        public ReferralLevelResponseModel? CurrentReferralLevel { get; set; }
        public ReferralLevelResponseModel? NextLevel { get; set; }
    }
}
