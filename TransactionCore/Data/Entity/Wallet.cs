using TransactionCore.Common.Enums;

namespace TransactionCore.Data.Entity
{
    public class Wallet
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public Guid PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }    
        public bool IsActive { get; set; }
        public WalletStatusEnum Status { get; set; }
        public decimal Balance { get; set; }
        public decimal? LastEntry { get; set; }
        public string LastTransactionId { get; set; }
    }
}
