namespace IdentityService.Models.RequestModels
{
    public class UserInfoUpdateRequestModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string TelegramUserName { get; set; }
    }
}
