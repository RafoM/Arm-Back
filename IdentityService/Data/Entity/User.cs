namespace IdentityService.Data.Entity
{
    public class User
    {
        // Primary key
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string TelegramUserName { get; set; }
        public string ReferralCode { get; set; }
        public string ?ProfileImageUrl { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public bool IsGmailAccount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
