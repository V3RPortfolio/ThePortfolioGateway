using Newtonsoft.Json;
using Ocelot.Configuration;
using Ocelot.Configuration.File;
using Ocelot.DependencyInjection;

namespace Infrastructure.Services
{
    internal enum Server
    {
        AuthenticationService,
    }

    internal class ServerSocket
    {
        public Server ServerAddress { get; set; }
        public int ServerPort { get; set; } = 8080;
    }

    internal class MappedServerAddress
    {
        public string Address { get; set;  } = "localhost";
        public int Port { get; set; } = 8080;

        public string? Scheme { get; set; } = "http";
    }

    internal class OcelotRoute
    {
        public List<ServerSocket> Servers { get; set; } = new List<ServerSocket>();
        public List<FileRoute> Routes { get; set; } = new List<FileRoute>();
    }

    public class OcelotConfiguration
    {
        private List<OcelotRoute> Routes = new List<OcelotRoute>()
        {
            new OcelotRoute
            {
                Servers = new List<ServerSocket>() {
                    new ServerSocket() { ServerAddress = Server.AuthenticationService }
                },
                Routes = new List<FileRoute>()
                {
                    new FileRoute()
                    {
                        DownstreamPathTemplate = "/admin/{everything}",
                        UpstreamPathTemplate = "/admin/{everything}",
                        UpstreamHttpMethod = new List<string>() { "Get", "Post", "Put", "Delete" },

                        // AuthenticationOptions = new FileAuthenticationOptions()
                        // {
                        //     // JWT Authentication
                        //     AuthenticationProviderKeys = new[] { "Bearer" }
                        // }

                    },
                    
                    new FileRoute()
                    {
                        DownstreamPathTemplate = "/api/auth/v1/{everything}",
                        UpstreamPathTemplate = "/auth/{everything}",
                        UpstreamHttpMethod = new List<string>() { "Get", "Post", "Put", "Delete" },
                    }
                }
            },
        };

        List<FileRoute> GenerateRoutes(IDictionary<string, MappedServerAddress> serverAddressMap)
        {
            var routes = new List<FileRoute>();

            foreach (var route in Routes)
            {
                var hosts = new List<FileHostAndPort>();
                var scheme = "http";
                foreach (var server in route.Servers)
                {
                    if (serverAddressMap.TryGetValue(server.ServerAddress.ToString(), out var address))
                    {
                        hosts.Add(new FileHostAndPort(address.Address, address.Port));
                        if(address.Scheme?.ToLower() == "https")
                        {
                            scheme = address.Scheme;
                        }
                    }
                }

                if (hosts.Count == 0)
                {
                    continue;
                }

                foreach (var fileRoute in route.Routes)
                {
                    fileRoute.DownstreamHostAndPorts = hosts;
                    if (fileRoute.DownstreamScheme == null)
                    {
                        fileRoute.DownstreamScheme = scheme;
                    }

                    if (fileRoute.UpstreamHttpMethod == null || fileRoute.UpstreamHttpMethod.Count == 0)
                    {
                        fileRoute.UpstreamHttpMethod = new List<string>() { "Get", "Post", "Put", "Delete", "Patch", "Options" };
                    }
                    
                    if(fileRoute.LoadBalancerOptions == null)
                    {
                        fileRoute.LoadBalancerOptions = new FileLoadBalancerOptions()
                        {
                            Type = "RoundRobin"
                        };
                    }
                    
                    routes.Add(fileRoute);
                }

            }

            return routes;
        }

        FileGlobalConfiguration GenerateGlobalConfiguration(string applicationHost)
        {
            return new FileGlobalConfiguration()
            {
                BaseUrl = applicationHost
            };
        }
        
        IDictionary<string, object> GetAppSettings(string appSettingsFilePath)
        {
            if (!File.Exists(appSettingsFilePath))
            {
                return new Dictionary<string, object>();
            }

            var json = File.ReadAllText(appSettingsFilePath);
            var appSettings = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            if (appSettings == null)
            {
                return new Dictionary<string, object>();
            }

            return appSettings;
        }

        public void Configure(IConfigurationBuilder config, string appSettingsFilePath, string applicationHost)
        {
            var appSettings = GetAppSettings(appSettingsFilePath);
            if (appSettings.Count == 0)
            {
                return;
            }
            
            var gatewayServices = new Dictionary<string, MappedServerAddress>();
            
            if(appSettings.ContainsKey("GATEWAY_SERVICES"))
            {
                var gatewayServicesJson = appSettings["GATEWAY_SERVICES"]?.ToString() ?? "{}";
                gatewayServices = JsonConvert.DeserializeObject<Dictionary<string, MappedServerAddress>>(gatewayServicesJson) ?? new Dictionary<string, MappedServerAddress>();
            }

            var routes = GenerateRoutes(gatewayServices);
            var globalConfiguration = GenerateGlobalConfiguration(applicationHost);

            var fileConfig = new FileConfiguration()
            {
                Routes = routes,
                GlobalConfiguration = globalConfiguration
            };

            config.AddOcelot(fileConfig, reloadOnChange: true);
        }
    }
}