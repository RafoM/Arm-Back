namespace IdentityService.Data.Entity
{
    public class PasswordResetToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string TokenHash { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedDate { get; set; }

        public User User { get; set; }
    }
}
