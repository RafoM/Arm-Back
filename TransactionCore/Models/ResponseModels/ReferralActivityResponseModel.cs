namespace TransactionCore.Models.ResponseModels
{
    public class ReferralActivityResponseModel
    {
        public string ReferredUser { get; set; }
        public decimal? Commission { get; set; }
        public string Action { get; set; }
        public DateTime ActionDate { get; set; } 
    }
}
