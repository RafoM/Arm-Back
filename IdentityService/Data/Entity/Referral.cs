namespace IdentityService.Data.Entity
{
    public class Referral
    {
        public Guid Id { get; set; }
        public Guid ReferrerUserId { get; set; } 
        public Guid ReferredUserId { get; set; } 
        public DateTime CreatedAt { get; set; }

        public User Referrer { get; set; }
        public User Referred { get; set; }
    }
}
