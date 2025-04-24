namespace TransactionCore.Data.Entity
{
    public class SubscriptionUsage
    {
        public Guid Id { get; set; }

        public Guid UserInfoId { get; set; }                  
        public Guid SubscriptionPackageId { get; set; }       
        public DateTime ActivatedAt { get; set; }             
        public DateTime? ExpiresAt { get; set; }              
        public bool IsActive { get; set; } = true;            
        public int? PromoBonusDays { get; set; }              

        public UserInfo UserInfo { get; set; } = null!;
        public SubscriptionPackage SubscriptionPackage { get; set; } = null!;
    }
}
