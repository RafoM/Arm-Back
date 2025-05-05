namespace TransactionCore.Services.Interface
{
    public interface IReferralRoleRewardSerice
    {
        Task<decimal> GetReferralRewardPercentageAsync(Guid? referrerId, int referralCount);
    }
}
