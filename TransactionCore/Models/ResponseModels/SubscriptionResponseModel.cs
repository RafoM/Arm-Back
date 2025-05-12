namespace TransactionCore.Models.ResponseModels
{
    public class SubscriptionResponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }
        public decimal FinalPrice { get; set; }
        public float Discount { get; set; }
        public string Currency { get; set; }
    }
}
