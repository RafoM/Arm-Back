namespace TransactionCore.Data.Entity
{
    public class ReferralRoleRewardConfig
    {
        public Guid Id { get; set; }
        public string Role { get; set; }

        public decimal? FixedPercentage { get; set; }
        public bool UseReferralLevels { get; set; }
    }
}
