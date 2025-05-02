using TransactionCore.Common.Enums;

namespace TransactionCore.Models.RequestModels
{
    public class PaymentMethodUpdateModel
    {
        public int Id { get; set; }
        public Guid CryptoId { get; set; }
        public Guid NetworkId { get; set; }
        public decimal TransactionFee { get; set; }
        public bool Status { get; set; }
        public string Note { get; set; }
    }
}
