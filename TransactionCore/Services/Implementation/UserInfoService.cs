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
        public UserInfoService(TransactionCoreDbContext dbContext, IAuthService authService, IConfiguration config, IHttpClientFactory clientFactory)
        {
            _dbContext = dbContext;
            _authService = authService;
            _config = config;
            _clientFactory = clientFactory;
        }

        public async Task<decimal> GetUserBalanceAsync(Guid userId)
        {
            var userFinance = await _dbContext.UserInfos.FirstOrDefaultAsync(x => x.UserId == userId);
            if (userFinance == null) { await CreateUserinfoAsync(userId); return 0; }
            return userFinance.Balance;
        }
        public async Task<UserInfo> GetUserInfo(Guid userId)
        {
            return await _dbContext.UserInfos.FirstOrDefaultAsync(x => x.UserId == userId);
        }
        public async Task<UserInfo> CreateUserinfoAsync(Guid userId, string? promoCode = null, Guid? referrerId = null)
        {
            var userFinance = new UserInfo 
            {
                UserId = userId,
                Balance = 0,
                ReferrerId = referrerId
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
            await _dbContext.AddAsync(userFinance);
            await _dbContext.SaveChangesAsync();

            return userFinance;
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
