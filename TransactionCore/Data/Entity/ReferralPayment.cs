namespace TransactionCore.Data.Entity
{
    public class ReferralPayment
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public Payment Payment { get; set; }
        public Guid ReferrerUserInfoId { get; set; }
        public UserInfo ReferrerUserInfo { get; set; }
        public decimal Commission { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
