using TransactionCore.Common.Enums;

namespace TransactionCore.Data.Entity
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid UserFinanceId { get; set; }
        public Guid? WalletId { get; set; }
        public Wallet Wallet { get; set; }
        public PaymentStatusEnum Status { get; set; }   
        public decimal ExpectedFee { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
