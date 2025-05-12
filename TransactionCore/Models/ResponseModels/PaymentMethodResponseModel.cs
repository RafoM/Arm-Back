using TransactionCore.Common.Enums;

namespace TransactionCore.Models.ResponseModels
{
    public class PaymentMethodResponseModel
    {
        public Guid Id { get; set; }
        public decimal TransactionFee { get; set; }
        public string Note { get; set; }
        public CryptoResponseModel Crypto { get; set; }
        public NetworkResponseModel Network { get; set; }
    }
}
