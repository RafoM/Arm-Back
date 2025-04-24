namespace TransactionCore.Data.Entity
{
    public class UserInfo
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? ReferrerId { get; set; }
        public decimal Balance { get; set; }
        public decimal ReferalBalance { get; set; }
        public Guid? ExpectedPaymentId { get; set; }
        public Guid? AmountWalletId { get; set; }
        public bool IsSubscribed { get; set; }
    }
}
