namespace IdentityService.Data.Entity
{
    public class RefreshToken
    {
        // Primary key
        public Guid Id { get; set; }          
        // Foreign key to User
        public Guid UserId { get; set; }      
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedDate { get; set; }
        public User User { get; set; }
    }
}
