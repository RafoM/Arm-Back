using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;

namespace ApiGateway
{
    public static class OcelotConfigGenerator
    {
        public static async Task GenerateOcelotConfigAsync()
        {
            var swaggerUrls = Environment.GetEnvironmentVariables()
                .Cast<DictionaryEntry>()
                .Where(e => e.Key is string key && key.StartsWith("SWAGGER_URL_"))
                .ToDictionary(e => ((string)e.Key).Replace("SWAGGER_URL_", "").ToLower(), e => e.Value!.ToString()!);

            var routes = new JArray();
            var swaggerEndpoints = new JArray();

            foreach (var (key, swaggerUrl) in swaggerUrls)
            {
                using var client = new HttpClient();
                var swaggerJson = await client.GetStringAsync(swaggerUrl);
                var swagger = JObject.Parse(swaggerJson);
                var paths = swagger["paths"];

                foreach (var pathItem in paths!.Children<JProperty>())
                {
                    var path = pathItem.Name;
                    foreach (var method in pathItem.Value.Children<JProperty>())
                    {
                        var route = new JObject
                        {
                            ["DownstreamPathTemplate"] = $"/api{path}",
                            ["DownstreamScheme"] = "https",
                            ["DownstreamHostAndPorts"] = new JArray
                    {
                        new JObject
                        {
                            ["Host"] = new Uri(swaggerUrl).Host,
                            ["Port"] = 443
                        }
                    },
                            ["UpstreamPathTemplate"] = $"/{key}{path}",
                            ["UpstreamHttpMethod"] = new JArray(method.Name.ToUpperInvariant()),
                            ["SwaggerKey"] = key
                        };

                        if (path.Contains("/admin") || path.Contains("/role"))
                        {
                            route["AuthenticationOptions"] = new JObject
                            {
                                ["AuthenticationProviderKey"] = "Bearer",
                                ["AllowedScopes"] = new JArray()
                            };
                            route["RouteClaimsRequirement"] = new JObject
                            {
                                ["role"] = "Admin"
                            };
                        }

                        routes.Add(route);
                    }
                }

                swaggerEndpoints.Add(new JObject
                {
                    ["Key"] = key,
                    ["Config"] = new JArray
            {
                new JObject
                {
                    ["Name"] = $"{char.ToUpper(key[0]) + key[1..]}Service",
                    ["Version"] = "v1",
                    ["Url"] = swaggerUrl
                }
            }
                });
            }

            var baseUrl = Environment.GetEnvironmentVariable("API_GATEWAY_URL") ?? "https://apigateway.local";

            var config = new JObject
            {
                ["Routes"] = routes,
                ["SwaggerEndPoints"] = swaggerEndpoints,
                ["GlobalConfiguration"] = new JObject
                {
                    ["BaseUrl"] = baseUrl
                }
            };

            File.WriteAllText("ocelot.json", JsonConvert.SerializeObject(config, Formatting.Indented));
            Console.WriteLine("✅ ocelot.json generated.");
        }
    }
}
