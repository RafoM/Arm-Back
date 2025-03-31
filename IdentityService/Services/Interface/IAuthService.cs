using IdentityService.Models.RequestModels;
using Microsoft.AspNetCore.Identity.Data;

namespace IdentityService.Services.Interface
{
    public interface IAuthService
    {
        Task<(string accessToken, string refreshToken)> RegisterAsync(RegisterRequestModel request);
        Task<(string accessToken, string refreshToken)> LoginAsync(LoginRequestModel request);
        Task<(string accessToken, string refreshToken)> GoogleRegistrationAsync(string idToken);
        Task<(string accessToken, string refreshToken)> GoogleLoginAsync(string idToken);
        Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string token);
        Task LogoutAsync(string refreshToken);
        Task ForgotPasswordAsync(ForgotPasswordRequestModel requestModel);
        Task ResetPasswordAsync(string jwtResetToken, ResetPasswordRequestModel requestModel);
    }
}
