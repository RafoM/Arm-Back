using Arbito.Shared.Contracts.Transaction;
using Google.Apis.Auth;
using IdentityService.Common.Helpers;
using IdentityService.Data;
using IdentityService.Data.Entity;
using IdentityService.Models.ConfigModels;
using IdentityService.Models.RequestModels;
using IdentityService.Services.Interface;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace IdentityService.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IdentityDbContext _dbContext;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly JwtSettingsConfigModel _jwtSettings;
        private readonly IEmailService _emailService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<AuthService> _logger;


        public AuthService(IdentityDbContext dbContext, IOptions<JwtSettingsConfigModel> jwtSettings, ITokenService tokenService, IConfiguration configuration, IEmailService emailService, IPublishEndpoint publishEndpoint = null, ILogger<AuthService> logger = null)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
            _configuration = configuration;
            _jwtSettings = jwtSettings.Value;
            _emailService = emailService;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<(string accessToken, string refreshToken)> RegisterAsync(RegisterRequestModel request)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email))
                throw new Exception("Email already in use.");

            if (request.Password != request.ConfirmPassword)
                throw new Exception("Passwords do not match.");

            var userRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "User")
                          ?? throw new Exception("Default user role not found.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var newUserId = Guid.NewGuid();

            var newUser = new User
            {
                Id = newUserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordHash,
                Country = request.Country,
                PhoneNumber = request.PhoneNumber,
                TelegramUserName = request.TelegramUserName,
                RoleId = userRole.Id,
                CreatedDate = DateTime.UtcNow,
                IsEmailVerified = false,
                IsGmailAccount = false
            };

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            Guid? referrerId = null;
            if (!string.IsNullOrWhiteSpace(request.ReferralCode))
            {
                referrerId = await GetReferrerIdFromCode(request.ReferralCode);
                var referrer = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == referrerId);

                if (referrer != null)
                {
                    referrerId = referrer.Id;
                    _dbContext.Referrals.Add(new Referral
                    {
                        Id = Guid.NewGuid(),
                        ReferrerUserId = referrer.Id,
                        ReferredUserId = newUserId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Sending user info to ContentService");

            await _publishEndpoint.Publish<ICreateUserInfoRequest>(new
            {
                UserId = newUser.Id,
                ReferrerId = referrerId,
                PromoCode = request.PromoCode,
                Email = request.Email
            });
            _logger.LogInformation("User info sent to ContentService");

            var refreshToken = _tokenService.GenerateRefreshToken();
            _dbContext.RefreshTokens.Add(new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = newUserId,
                Token = refreshToken,
                ExpireDate = DateTime.UtcNow.AddDays(7),
                CreatedDate = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            var accessToken = _tokenService.GenerateAccessToken(newUser);
            return (accessToken, refreshToken);
        }
        private async Task<Guid?> GetReferrerIdFromCode(string referralCode)
        {
            try
            {
                var referrerId = ReferralHelper.DecryptReferralCode(referralCode);

                var exists = await _dbContext.Users.AnyAsync(u => u.Id == referrerId);
                return exists ? referrerId : null;
            }
            catch
            {
                return null; 
            }
        }
        public string GetReferralCode(Guid userId)
        {
            return ReferralHelper.EncryptUserId(userId);
        }

        public async Task<(string accessToken, string refreshToken)> LoginAsync(LoginRequestModel request)
        {
            var user = await _dbContext.Users.Include(x => x.Role).FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                throw new Exception("Invalid credentials.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new Exception("Invalid credentials.");

            var accessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = newRefreshToken,
                ExpireDate = DateTime.UtcNow.AddDays(7),
                CreatedDate = DateTime.UtcNow
            };

            _dbContext.RefreshTokens.Add(refreshTokenEntity);
            await _dbContext.SaveChangesAsync();

            return (accessToken, newRefreshToken);
        }

        public async Task<(string accessToken, string refreshToken)> GoogleRegistrationAsync(string idToken)
        {
            var payload = await ValidateGoogleToken(idToken);

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);
            if (user != null)
            {
                return await GoogleLoginAsync(idToken);
            }

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Email = payload.Email,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                IsGmailAccount = true,
                PasswordHash = null,
                RoleId = 2,
                CreatedDate = DateTime.UtcNow,
                IsEmailVerified = true
            };

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            var (accessToken, refreshToken) = await GenerateTokenPairForUser(newUser);
            return (accessToken, refreshToken);
        }

        public async Task<(string accessToken, string refreshToken)> GoogleLoginAsync(string idToken)
        {
            var payload = await ValidateGoogleToken(idToken);

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);
            if (user == null)
            {
                throw new Exception("User with this Google account does not exist. Please register first.");
            }

            return await GenerateTokenPairForUser(user);
        }

        private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleToken(string idToken)
        {
            try
            {
                return await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings());
            }
            catch (InvalidJwtException)
            {
                throw new Exception("Invalid or expired Google token.");
            }
        }

        private async Task<(string accessToken, string refreshToken)> GenerateTokenPairForUser(User user)
        {
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = refreshToken,
                ExpireDate = DateTime.UtcNow.AddDays(7),
                CreatedDate = DateTime.UtcNow
            };
            _dbContext.RefreshTokens.Add(refreshTokenEntity);

            await _dbContext.SaveChangesAsync();
            return (accessToken, refreshToken);
        }

        public async Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string token)
        {
            var refreshToken = await _dbContext.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked);

            if (refreshToken == null || refreshToken.ExpireDate < DateTime.UtcNow)
                throw new Exception("Invalid or expired refresh token.");

            refreshToken.IsRevoked = true;
            _dbContext.RefreshTokens.Update(refreshToken);

            var user = refreshToken.User;
            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = newRefreshToken,
                ExpireDate = DateTime.UtcNow.AddDays(7),
                CreatedDate = DateTime.UtcNow
            };
            _dbContext.RefreshTokens.Add(refreshTokenEntity);

            await _dbContext.SaveChangesAsync();

            return (newAccessToken, newRefreshToken);
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var token = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (token == null) return;

            token.IsRevoked = true;
            _dbContext.RefreshTokens.Update(token);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequestModel requestModel)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == requestModel.Email);
            if (user == null) return;

            var resetPasswordBaseUrl = _configuration["JwtSettings:ResetPasswordBaseUrl"];

            var resetLink = new StringBuilder(resetPasswordBaseUrl);
            var rawToken = GeneratePasswordResetToken(user);
            resetLink.Append(rawToken);

            string subject = "Reset Your Password";
            string htmlContent = $@"<p>Hello {user.FirstName},</p>
                                    <p>We received a request to reset your password. Please click the link below to reset your password:</p>
                                    <p><a href='{resetLink}'>Reset Password</a></p>
                                    <p>If you did not request a password reset, please ignore this email.</p>
                                    <p>Thanks,</p>
                                    <p>Your Support Team</p>";

            await _emailService.SendEmailAsync(user.Email, subject, htmlContent);
        }
        

        public async Task ResetPasswordAsync(string jwtResetToken, ResetPasswordRequestModel request)
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                throw new Exception("New password and confirm password do not match.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            try
            {
                var principal = tokenHandler.ValidateToken(jwtResetToken, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                var hasResetClaim = principal.Claims.Any(c => c.Type == "ResetPassword" && c.Value == "true");
                if (!hasResetClaim)
                {
                    throw new SecurityTokenException("Invalid reset token (missing claim).");
                }

                var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
                if (userIdClaim == null) throw new Exception("Token missing user ID.");

                var userId = Guid.Parse(userIdClaim.Value);

                var user = await _dbContext.Users.FindAsync(userId);

                if (user == null)
                    throw new Exception("User not found.");
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (SecurityTokenException ex)
            {
                throw new Exception("Invalid or expired reset token.");
            }
        }


        private string GeneratePasswordResetToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var claims = new[]
            {
                new Claim("ResetPassword", "true"),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
