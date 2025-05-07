using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace ApiGateway
{
    public static class OcelotConfigGenerator
    {
        public static async Task GenerateOcelotConfigAsync(IConfiguration configuration)
        {
            var swaggerSourcesSection = configuration.GetSection("SwaggerSources");
            var swaggerUrls = swaggerSourcesSection
                .GetChildren()
                .ToDictionary(
                    child => child.Key.ToLowerInvariant(),
                    child => child.Value ?? string.Empty
                );

            if (!swaggerUrls.Any())
                throw new Exception("No Swagger source URLs found in environment variables.");

            var routesMap = new Dictionary<string, JObject>();
            var swaggerEndpoints = new JArray();
            var httpClient = new HttpClient();

            foreach (var (key, swaggerUrl) in swaggerUrls)
            {
                string swaggerJson;
                try
                {
                    swaggerJson = await httpClient.GetStringAsync(swaggerUrl);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Failed to fetch Swagger from {swaggerUrl}: {ex.Message}");
                    Console.ResetColor();
                    continue;
                }

                JObject swagger;
                try
                {
                    swagger = JObject.Parse(swaggerJson);
                }
                catch (JsonException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Failed to parse Swagger JSON from {swaggerUrl}: {ex.Message}");
                    Console.ResetColor();
                    continue;
                }

                var paths = swagger["paths"] as JObject;
                if (paths == null)
                    continue;

                foreach (var pathItem in paths.Properties())
                {
                    var path = pathItem.Name;
                    var upstreamPath = $"/{key}{path}";
                    var downstreamPath = path.StartsWith("/api") ? path : $"/api{path}";
                    var routeKey = $"{key}:{path}";

                    if (!routesMap.TryGetValue(routeKey, out var route))
                    {
                        route = new JObject
                        {
                            ["DownstreamPathTemplate"] = downstreamPath,
                            ["DownstreamScheme"] = "https",
                            ["DownstreamHostAndPorts"] = new JArray
                            {
                                new JObject
                                {
                                    ["Host"] = new Uri(swaggerUrl).Host,
                                    ["Port"] = 443
                                }
                            },
                            ["UpstreamPathTemplate"] = upstreamPath,
                            ["UpstreamHttpMethod"] = new JArray(),
                            ["SwaggerKey"] = key
                        };

                        if (path.Contains("/admin", StringComparison.OrdinalIgnoreCase) || path.Contains("/role", StringComparison.OrdinalIgnoreCase))
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

                        routesMap[routeKey] = route;
                    }

                    var httpMethods = (JArray)route["UpstreamHttpMethod"];
                    foreach (var method in pathItem.Value.Children<JProperty>())
                    {
                        var httpMethod = method.Name.ToUpperInvariant();
                        if (!httpMethods.Contains(httpMethod))
                            httpMethods.Add(httpMethod);
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
                ["Routes"] = new JArray(routesMap.Values),
                ["SwaggerEndPoints"] = swaggerEndpoints,
                ["GlobalConfiguration"] = new JObject
                {
                    ["BaseUrl"] = baseUrl
                }
            };

            const string outputPath = "ocelot.json";
            File.WriteAllText(outputPath, JsonConvert.SerializeObject(config, Formatting.Indented));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{outputPath} generated successfully.");
            Console.ResetColor();
        }
    }
}
