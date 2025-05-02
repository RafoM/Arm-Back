using TransactionCore.Common.Enums;

namespace TransactionCore.Data.Entity
{
    public class ReferralWithdrawal
    {
        public Guid Id { get; set; }

        public Guid ReferrerUserInfoId { get; set; }
        public UserInfo ReferrerUserInfo { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Network { get; set; }

        public string ToAddress { get; set; }

        public ReferralWithdrawalStatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }

        public string? TxHash { get; set; }
    }

}
