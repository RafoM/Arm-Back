namespace TransactionCore.Models.RequestModels
{
    public class PaymentDetailsRequestModel
    {
        public Guid SubscriptionPackageId { get; set; }
        public string? PromoCode { get; set; }
        public Guid CryptoId { get; set; }
        public Guid NetworkId { get; set; }
    }
}
