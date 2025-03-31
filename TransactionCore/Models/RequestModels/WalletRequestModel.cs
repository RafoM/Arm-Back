using TransactionCore.Common.Enums;

namespace TransactionCore.Models.RequestModels
{
    public class WalletRequestModel
    {
        public string Address { get; set; }
        public Guid PaymentMethodId { get; set; }
        public bool IsActive { get; set; }
        public decimal Balance { get; set; }
    }
}
