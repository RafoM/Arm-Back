using TransactionCore.Common.Enums;

namespace TransactionCore.Data.Entity
{
    public class PaymentMethod
    {
        public Guid Id { get; set; }
        public Guid CryptoId { get; set; }
        public Crypto Crypto { get; set; }
        public Guid NetworkId { get; set; }
        public Network Network { get; set; }
        public decimal TransactionFee { get; set; }
        public PaymentMethodStatusEnum Status { get; set; }
        public string Note { get; set; }
    }
}
