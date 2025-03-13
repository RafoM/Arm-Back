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
    }
}
