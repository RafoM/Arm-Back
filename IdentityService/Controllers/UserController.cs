using IdentityService.Models.RequestModels;
using IdentityService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityService.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private Guid UserId => Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get a user's info by ID
        /// </summary>
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetUserInfo(Guid userId)
        {
            var user = await _userService.GetUserInfoAsync(userId);
            return Ok(user);
        }

        /// <summary>
        /// Get all users
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Update user info
        /// </summary>
        [HttpPut("{userId:guid}")]
        public async Task<IActionResult> UpdateUserInfo(Guid userId, [FromBody] UserInfoUpdateRequestModel requestModel)
        {
            try
            {
                await _userService.UpdateUserInfoAsync(userId, requestModel);
                return Ok(new { message = "User info successfully updated" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update user role
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{userId:guid}/role/{newRoleId:int}")]
        public async Task<IActionResult> UpdateUserRole(Guid userId, int newRoleId)
        {
            var user = await _userService.UpdateUserRoleAsync(userId, newRoleId);
            return Ok(user);
        }
        /// <summary>
        /// Uploads a profile image for a user. Returns the S3 URL.
        /// </summary>
        [HttpPost("profile-image")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            var imageUrl = await _userService.UpdateUserProfileImageAsync(UserId, file);
            return Ok(new { imageUrl });
        }

        /// <summary>
        /// Endpoint to send a verification email for the specified user.
        /// </summary>
        [HttpPost("send-verification-email/{userId:guid}")]
        public async Task<IActionResult> SendVerificationEmail(Guid userId)
        {
            await _userService.SendVerificationEmailAsync(userId);
            return Ok(new { message = "Verification email sent." });
        }

        /// <summary>
        /// Updates the password for the currently authenticated user.
        /// </summary>
        /// <param name="request">Contains the current and new password.</param>
        /// <returns>A confirmation message.</returns>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _userService.ChangePasswordAsync(UserId, request);
                return Ok(new { message = "Password changed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint to verify an email using a token.
        /// </summary>
        [HttpGet("verify")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required.");

            var isVerified = await _userService.VerifyEmailAsync(token);
            if (!isVerified)
                return BadRequest(new { message = "Invalid or expired token." });

            return Ok(new { message = "Email verified successfully." });
        }

        /// <summary>
        /// Checks if the currently authenticated user's email address has been verified.
        /// </summary>
        /// <returns>
        /// An HTTP 200 OK response containing a boolean value:
        /// true if the email is verified, false otherwise.
        /// </returns>
        [HttpGet("is-email-verified")]
        public async Task<IActionResult> IsEmailVerified()
        {
            return Ok(await _userService.IsEmailVerifiedAsync(UserId));
        }
    }
}
