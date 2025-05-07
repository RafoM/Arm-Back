using TransactionCore.Common.Enums;

namespace TransactionCore.Data.Entity
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid UserInfoId { get; set; }
        public UserInfo UserInfo { get; set; }
        public Guid? WalletId { get; set; }
        public Wallet Wallet { get; set; }
        public PaymentStatusEnum Status { get; set; }
        public Guid? PromoId { get; set; }
        public decimal ExpectedFee { get; set; }
        public decimal? PayedFee { get; set; }
        public Guid SubscriptionPackageId { get; set; }
        public SubscriptionPackage SubscriptionPackage { get; set; }
        public string TxHash { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
