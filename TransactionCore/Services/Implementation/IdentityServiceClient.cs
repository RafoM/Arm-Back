using System.Net.Http.Headers;
using TransactionCore.Services.Interface;

namespace TransactionCore.Services.Implementation
{
    public class IdentityServiceClient : IIdentityServiceClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public IdentityServiceClient(IHttpClientFactory clientFactory, IAuthService authService, IConfiguration config)
        {
            _clientFactory = clientFactory;
            _authService = authService;
            _config = config;
        }

        public async Task UpdateUserRoleAsync(string userId, string newRole)
        {
            var identityBaseUrl = _config["Services:IdentityServiceUrl"];
            var token = await _authService.AuthorizeMicroserviceAsync();

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(identityBaseUrl!);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                UserId = userId,
                NewRole = newRole
            };

            var response = await client.PostAsJsonAsync("internal/update-role", payload);
            response.EnsureSuccessStatusCode();
        }
    }
}
