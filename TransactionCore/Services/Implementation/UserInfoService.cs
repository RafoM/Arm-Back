using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using TransactionCore.Data;
using TransactionCore.Data.Entity;
using TransactionCore.Services.Interface;

namespace TransactionCore.Services.Implementation
{
    public class UserInfoService : IUserInfoService
    {
        private readonly TransactionCoreDbContext _dbContext;
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IReferralService _referralService;
        private readonly ILogger<UserInfoService> _logger;

        public UserInfoService(TransactionCoreDbContext dbContext, IAuthService authService, IConfiguration config, IHttpClientFactory clientFactory, IReferralService referralService, ILogger<UserInfoService> logger)
        {
            _dbContext = dbContext;
            _authService = authService;
            _config = config;
            _clientFactory = clientFactory;
            _referralService = referralService;
            _logger = logger;
        }

        public async Task AddReward(Guid userInfoId, decimal reward, Guid walletId)
        {
            var userInfo = await _dbContext.UserInfos.FirstOrDefaultAsync(x => x.Id == userInfoId);
            var referrer = await _dbContext.UserInfos.FirstOrDefaultAsync(x => x.UserId == userInfo.ReferrerId);
            referrer.ReferalBalance += reward;
             _dbContext.Update(userInfo);

            
            await _dbContext.SaveChangesAsync();
        }
      
        public async Task<UserInfo> GetUserInfo(Guid userId)
        {
            return await _dbContext.UserInfos.FirstOrDefaultAsync(x => x.UserId == userId);
        }
        public async Task<UserInfo> CreateUserinfoAsync(Guid userId, string email, string? promoCode = null, Guid? referrerId = null)
        {
            _logger.LogInformation("Create user info");

            var userInfo = new UserInfo 
            {
                UserId = userId,
                Balance = 0,
                ReferrerId = referrerId,
                DenormalizedEmail = DenormalizeEmailAddress(email),
                CreatedAt = DateTime.UtcNow,
            };
            if (!string.IsNullOrEmpty(promoCode)) 
            {
                var promoId = await _dbContext.UserInfos.Where(x => x.Equals(promoCode)).Select(p => p.Id).FirstOrDefaultAsync();
                if (promoId != null)
                {
                    var promoUsage = new PromoUsage
                    {
                        PromoId = promoId,
                        UserId = userId,
                        UsedAt = DateTime.UtcNow
                    };

                    await _dbContext.AddAsync(promoUsage);
                }
            }
            await _dbContext.AddAsync(userInfo);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("User info created");

            if (referrerId != null)
            {
                await _referralService.CreateReferralActivityAsync(userInfo.Id, Common.Enums.ReferralActionTypeEnum.Registration);

            }

            return userInfo;
        }

        private string DenormalizeEmailAddress(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                throw new ArgumentException("Invalid email address.", nameof(email));

            var parts = email.Split('@');
            var local = parts[0];
            var domain = parts[1];

            if (local.Length <= 5)
                return new string('*', local.Length) + "@" + domain;

            var prefix = local.Substring(0, 3);
            var suffix = local.Substring(local.Length - 2);
            var middle = new string('*', local.Length - prefix.Length - suffix.Length);

            return $"{prefix}{middle}{suffix}@{domain}";
        }

        //public async Task UpdateUserRoleAsync(Guid userId, int roleId)
        //{
        //    var token = await _authService.AuthorizeMicroserviceAsync();
        //    var client = _clientFactory.CreateClient("IdentityService");
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    var url = _config["ChangeRoleUrl"];

        //    var payload = new
        //    {
        //        UserId = userId,
        //        RoleId = roleId
        //    };

        //    var content = new StringContent(
        //        JsonSerializer.Serialize(payload),
        //        Encoding.UTF8,
        //        "application/json"
        //    );

        //    var response = await client.PutAsync(url, content);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        var error = await response.Content.ReadAsStringAsync();
        //        throw new Exception($"Failed to update user role: {response.StatusCode} - {error}");
        //    }
        //}
    }
}
