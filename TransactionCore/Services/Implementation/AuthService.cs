using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using TransactionCore.Services.Interface;

namespace TransactionCore.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;

        public AuthService(IConfiguration config, IHttpClientFactory clientFactory)
        {
            _config = config;
            _clientFactory = clientFactory;
        }

        public async Task<string> AuthorizeMicroserviceAsync()
        {
            var clientId = _config["MicroserviceAuth:ClientId"];
            var clientSecret = _config["MicroserviceAuth:ClientSecret"];
            var tokenUrl = _config["MicroserviceAuth:TokenUrl"];

            var client = _clientFactory.CreateClient();
            var requestData = new Dictionary<string, string>
            {
                ["client_id"] = clientId!,
                ["client_secret"] = clientSecret!,
                ["grant_type"] = "client_credentials"
            };

            var response = await client.PostAsync(tokenUrl, new FormUrlEncodedContent(requestData));
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("access_token").GetString()!;
        }
       
    }
}
