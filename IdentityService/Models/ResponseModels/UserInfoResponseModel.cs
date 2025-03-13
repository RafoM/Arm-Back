
namespace IdentityService.Models.ResponseModels
{
    public class UserInfoResponseModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string TelegramUserName { get; set; }
        public string ProfileImageUrl { get; set; }
        public string RoleName { get; set; }
    }
}
