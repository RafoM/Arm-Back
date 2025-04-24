namespace TransactionCore.Models.ResponseModels
{
    public class TransactionCheckResultModel
    {
        public string TransactionId { get; set; }
        public bool Found { get; set; }
        public bool Confirmed { get; set; }
        public string Token { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
        public bool BelongsToUser { get; set; }
    }
}
