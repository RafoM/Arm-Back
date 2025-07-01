namespace IdentityService.Models.ConfigModels
{
    public class JwtSettingsConfigModel
    {
        public string SecretKey { get; set; }
        public int AccessTokenLifetimeMinutes { get; set; }
        public string Issuer { get; set; }
        public List<string> Audiences { get; set; } = new();
    }
}
