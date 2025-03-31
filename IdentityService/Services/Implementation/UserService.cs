using Google.Cloud.Storage.V1;
using IdentityService.Data;
using IdentityService.Data.Entity;
using IdentityService.Models.RequestModels;
using IdentityService.Models.ResponseModels;
using IdentityService.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IdentityDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        //private readonly StorageClient _storageClient;

        //, StorageClient storageClient

        public UserService(IdentityDbContext dbContext, IConfiguration configuration, IEmailService emailService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _emailService = emailService;
           // _storageClient = storageClient;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _dbContext.Users
                                   .Include(u => u.Role)
                                   .ToListAsync();
        }

        public async Task UpdateUserInfoAsync(Guid userId, UserInfoUpdateRequestModel request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new Exception("User not found.");

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.Country = request.Country;
            user.UpdatedDate = DateTime.UtcNow;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsEmailVerifiedAsync(Guid userId)
        {
            return await _dbContext.Users.Where(y => y.Id == userId)
                                         .Select(x => x.IsEmailVerified)
                                         .FirstOrDefaultAsync();
        }

        public async Task SendVerificationEmailAsync(Guid userId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new Exception("User not found.");

            var token = Guid.NewGuid().ToString();
            user.VerificationToken = token;
            user.VerificationTokenExpiry = DateTime.UtcNow.AddHours(24);
            user.IsEmailVerified = false;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            var baseUrl = _configuration["BaseUrl"];
            var verificationUrl = $"{baseUrl}/api/user/verify?token={token}";

            var subject = "Please verify your email address";
            var htmlContent = $"<p>Please verify your email by clicking <a href='{verificationUrl}'>here</a>.</p>";

            await _emailService.SendEmailAsync(user.Email, subject, htmlContent);
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.VerificationToken == token
                                                                    && u.VerificationTokenExpiry > DateTime.UtcNow);
            if (user == null) return false;

            user.IsEmailVerified = true;
            user.VerificationToken = null;
            user.VerificationTokenExpiry = null;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<UserInfoResponseModel> GetUserInfoAsync(Guid userId)
        {
            var userInfo = await _dbContext.Users
                                           .Where(x => x.Id == userId)
                                           .Select(u => new UserInfoResponseModel
                                           {
                                               Id = u.Id,
                                               FirstName = u.FirstName,
                                               LastName = u.LastName,
                                               Email = u.Email,
                                               Country = u.Country,
                                               PhoneNumber = u.PhoneNumber,
                                               TelegramUserName = u.TelegramUserName,
                                               ProfileImageUrl = u.ProfileImageUrl,
                                               RoleName = u.Role != null ? u.Role.Name : null
                                           })
                                           .FirstOrDefaultAsync();

            if (userInfo == null) throw new Exception("User not found.");

            return userInfo;
        }

        public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequestModel requestModel)
        {
            if (requestModel.NewPassword != requestModel.ConfirmPassword)
            {
                throw new Exception("New password and confirm password do not match.");
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new Exception("User not found.");

            if (!BCrypt.Net.BCrypt.Verify(requestModel.CurrentPassword, user.PasswordHash))
                throw new Exception("Current password is incorrect.");

            if (string.IsNullOrWhiteSpace(requestModel.NewPassword) || requestModel.NewPassword.Length < 8)
                throw new Exception("New password must be at least 8 characters long.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(requestModel.NewPassword);
            user.UpdatedDate = DateTime.UtcNow;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }


        public async Task<User> UpdateUserRoleAsync(UpdateUserRoleRequestModel requestModel)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == requestModel.UserId);
            if (user == null) throw new Exception("User not found.");

            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == requestModel.RoleId);
            if (role == null) throw new Exception("Invalid role ID.");

            user.RoleId = requestModel.RoleId;
            user.UpdatedDate = DateTime.UtcNow;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<string> UpdateUserProfileImageAsync(Guid userId, IFormFile imageFile)
        {
            //var bucketName = _configuration["GCS:BucketName"];

            //var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            //if (user == null) throw new Exception("User not found.");
            //if (imageFile == null || imageFile.Length == 0) throw new Exception("Invalid image file.");
            //if (string.IsNullOrWhiteSpace(bucketName)) throw new Exception("GCS bucket name is not configured.");



            //var fileExtension = Path.GetExtension(imageFile.FileName);
            //var objectKey = $"profile-images/{userId}/{Guid.NewGuid()}{fileExtension}";

            //using (var stream = imageFile.OpenReadStream())
            //{
            //    await _storageClient.UploadObjectAsync(
            //        bucket: bucketName,
            //        objectName: objectKey,
            //        contentType: imageFile.ContentType,
            //        source: stream
            //    );
            //}

            //if (user.ProfileImageUrl != null)
            //{
            //    var prefix = $"https://storage.googleapis.com/{bucketName}/";
            //    var existingObjectKey = user.ProfileImageUrl.Replace(prefix, "");
            //    await _storageClient.DeleteObjectAsync(bucketName, existingObjectKey);
            //}

            //var gcsUrl = $"https://storage.googleapis.com/{bucketName}/{objectKey}";

            //user.ProfileImageUrl = gcsUrl;
            //_dbContext.Users.Update(user);
            //await _dbContext.SaveChangesAsync();

            //return gcsUrl;
            return "";
        }
    }
}

