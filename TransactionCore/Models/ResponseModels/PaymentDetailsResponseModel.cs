namespace TransactionCore.Models.ResponseModels
{
    public class PaymentDetailsResponseModel
    {
        public decimal Price { get; set; }
        public decimal Balance { get; set; }
        public decimal ExpectedFee { get; set; }
        public decimal TransactionFee { get; set; }
        public string WaletAddress { get; set; }
    }
}
