using IdentityService.Data.Entity;
using IdentityService.Models.RequestModels;
using IdentityService.Models.ResponseModels;

namespace IdentityService.Services.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<UserInfoResponseModel> GetUserInfoAsync(Guid userId);
        Task<User> UpdateUserRoleAsync(Guid userId, int newRoleId);
        Task UpdateUserInfoAsync(Guid userId, UserInfoUpdateRequestModel request);
        Task<string> UpdateUserProfileImageAsync(Guid userId, IFormFile imageFile);
        Task<bool> IsEmailVerifiedAsync(Guid userId);
        Task SendVerificationEmailAsync(Guid userId);
        Task ChangePasswordAsync(Guid userId, ChangePasswordRequestModel requestModel);
        Task<bool> VerifyEmailAsync(string token);
    }
}
