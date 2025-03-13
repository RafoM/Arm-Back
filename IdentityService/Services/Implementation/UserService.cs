using Amazon.S3;
using Amazon.S3.Model;
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
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;

        public UserService(IdentityDbContext dbContext, IAmazonS3 s3Client, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _s3Client = s3Client;
            _configuration = configuration;
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

        public async Task<UserInfoResponseModel> GetUserInfoAsync(Guid userId)
        {
            var userInfo = await _dbContext.Users
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
                                           .FirstOrDefaultAsync(x => x.Id == userId);

            if (userInfo == null) throw new Exception("User not found.");

            return userInfo;
        }

        public async Task<User> UpdateUserRoleAsync(Guid userId, int newRoleId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new Exception("User not found.");

            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == newRoleId);
            if (role == null) throw new Exception("Invalid role ID.");

            user.RoleId = newRoleId;
            user.UpdatedDate = DateTime.UtcNow;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<string> UpdateUserProfileImageAsync(Guid userId, IFormFile imageFile)
        {
            var bucketName = _configuration["S3:BucketName"];

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) throw new Exception("User not found.");
            if (imageFile == null || imageFile.Length == 0) throw new Exception("Invalid image file.");
            if (string.IsNullOrWhiteSpace(bucketName)) throw new Exception("S3 bucket name is not configured.");
            if (user.ProfileImageUrl != null) await _s3Client.DeleteObjectAsync(bucketName, user.ProfileImageUrl.Replace($"https://{bucketName}.s3.amazonaws.com/", ""));

            var fileExtension = Path.GetExtension(imageFile.FileName);
            var objectKey = $"profile-images/{userId}/{Guid.NewGuid()}{fileExtension}";

            using (var stream = imageFile.OpenReadStream())
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    InputStream = stream,
                    ContentType = imageFile.ContentType
                };

                // putRequest.CannedACL = S3CannedACL.PublicRead;

                var response = await _s3Client.PutObjectAsync(putRequest);

                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception("Failed to upload image to S3.");
            }

            var s3Url = $"https://{bucketName}.s3.amazonaws.com/{objectKey}";

            user.ProfileImageUrl = s3Url;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return s3Url;
        }
    }
}

