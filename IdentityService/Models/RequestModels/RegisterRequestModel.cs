namespace IdentityService.Models.RequestModels
{
    public class RegisterRequestModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string TelegramUserName { get; set; }
        public string ReferralCode { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
