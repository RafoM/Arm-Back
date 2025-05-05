namespace TransactionCore.Data.Entity
{
    public class UserInfo
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string DenormalizedEmail { get; set; }
        public Guid? ReferrerId { get; set; }
        public int ReferralPurchaseCount { get; set; }
        public int Visits {  get; set; }
        public decimal Balance { get; set; }
        public decimal ReferalBalance { get; set; }
        public Guid? ExpectedPaymentId { get; set; }
        public bool IsSubscribed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
