using TransactionCore.Common.Enums;

namespace TransactionCore.Data.Entity
{
    public class ReferralActivity
    {
        public Guid Id { get; set; }

        public Guid ReferredUserInfoId { get; set; }
        public UserInfo ReferredUserInfo { get; set; }

        public ReferralActionTypeEnum Action { get; set; } 

        public Guid? PaymentId { get; set; } 
        public Payment Payment { get; set; }

        public decimal? Commission { get; set; }

        public DateTime ActionDate { get; set; }
    }

}
