using Duende.IdentityServer.Models;
using Microsoft.Extensions.Configuration;

namespace IdentityService.IdentityServerConfig
{
    public static class IdentityServerDefinitions
    {
        public static IEnumerable<Client> GetClients(IConfiguration config)
        {
            var clientId = config["MicroserviceAuth:ClientId"] ?? "TransactionCore";
            var clientSecret = config["MicroserviceAuth:ClientSecret"] ?? "default-secret";

            return new List<Client>
            {
                new Client
                {
                    ClientId = clientId,
                    ClientSecrets = { new Secret(clientSecret.Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "identity-api" }
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes() => new List<ApiScope>
        {
            new ApiScope("identity-api", "Access to IdentityService APIs")
        };

        public static IEnumerable<ApiResource> GetApiResources() => new List<ApiResource>
        {
            new ApiResource("identity-api")
            {
                Scopes = { "identity-api" }
            }
        };

        public static IEnumerable<IdentityResource> GetIdentityResources() => new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };
    }
}
