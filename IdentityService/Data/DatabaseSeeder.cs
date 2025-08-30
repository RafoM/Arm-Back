using IdentityService.Data.Entity;

namespace IdentityService.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IdentityDbContext context, IConfiguration configuration)
        {
            var adminEmail = configuration["AdminSettings:Email"];
            var adminPassword = configuration["AdminSettings:Password"];

            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
            {
                throw new Exception("Admin email or password is not configured in environment variables.");
            }


            if (!context.Users.Any(u => u.Email.ToLower() == adminEmail.ToLower()))
            {
                var adminUser = new User
                {
                    FirstName = "Admin",
                    LastName = "User",
                    Email = adminEmail,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    RoleId = 1, 
                    CreatedDate = DateTime.UtcNow,
                    Id = Guid.NewGuid(),
                    
                    Country = "AM",                  // <-- REQUIRED
                    PhoneNumber = "+37400000000",    // <-- REQUIRED
                    TelegramUserName = "admin_bot",  // <-- REQUIRED
                    ProfileImageUrl = null,
                    IsGmailAccount = false,
                    IsEmailVerified = true,
                    UpdatedDate = null,
                    VerificationToken = null,
                    VerificationTokenExpiry = null
                };
                context.Users.Add(adminUser);
            }

            await context.SaveChangesAsync();
        }
    }
}
