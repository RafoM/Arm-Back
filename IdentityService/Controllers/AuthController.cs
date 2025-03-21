using IdentityService.Models.RequestModels;
using IdentityService.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user with email/password and returns JWT tokens.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (accessToken, refreshToken) = await _authService.RegisterAsync(request);
            return Ok(new { accessToken, refreshToken });
        }

        /// <summary>
        /// Logs in a user with email/password and returns JWT tokens.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
        {
            var (accessToken, refreshToken) = await _authService.LoginAsync(request);
            return Ok(new { accessToken, refreshToken });
        }

        /// <summary>
        /// Log in user via Google OAuth and returns JWT tokens.
        /// </summary>
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestModel requestModel)
        {
            var (accessToken, refreshToken) = await _authService.GoogleLoginAsync(requestModel.IdToken);
            return Ok(new { accessToken, refreshToken });
        }

        /// <summary>
        /// Register user via Google OAuth and returns JWT tokens.
        /// </summary>
        /// <param name="idToken"></param>
        /// <returns></returns>
        [HttpPost("google-register")]
        public async Task<IActionResult> GoogleRegister([FromBody] GoogleLoginRequestModel requestModel)
        {
            var (accessToken, refreshToken) = await _authService.GoogleRegistrationAsync(requestModel.IdToken);
            return Ok(new { accessToken, refreshToken });
        }

        /// <summary>
        /// Refreshes an expired or near-expired access token using a valid refresh token.
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestModel requestModel)
        {
            var (accessToken, newRefreshToken) = await _authService.RefreshTokenAsync(requestModel.RefreshToken);
            return Ok(new { accessToken, newRefreshToken });
        }

        /// <summary>
        /// Logs out a user by invalidating a given refresh token.
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestModel requestModel)
        {
            await _authService.LogoutAsync(requestModel.RefreshToken);
            return Ok(new { message = "Logged out" });
        }

        /// <summary>
        /// Initiates password reset by sending an email with a reset token/link.
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            await _authService.ForgotPasswordAsync(email);
            return Ok(new { message = "If the email is valid, a reset link will be sent." });
        }

        /// <summary>
        /// Resets the user password after clicking the link from the forgot-password email.
        /// </summary>
        [HttpPost("reset-password/{token}")]
        public async Task<IActionResult> ResetPassword(string token, [FromBody] ResetPasswordRequestModel requestModel)
        {
            await _authService.ResetPasswordAsync(token, requestModel);
            return Ok(new { message = "Password has been reset." });
        }
    }
}
