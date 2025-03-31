using TransactionCore.Common.Enums;

namespace TransactionCore.Models.ResponseModels
{
    public class PaymentMethodResponseModel
    {
        public Guid Id { get; set; }
        public Guid CryptoId { get; set; }
        public string CryptoName { get; set; }
        public Guid NetworkId { get; set; }
        public string NetworkName { get; set; }
        public decimal TransactionFee { get; set; }
        public string Note { get; set; }
    }
}
