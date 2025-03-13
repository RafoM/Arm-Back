using IdentityService.Data.Entity;
using System.Security.Claims;

namespace IdentityService.Services.Interface
{
    public interface ITokenService
    {
        string GenerateRefreshToken();
        string GenerateAccessToken(User user);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
