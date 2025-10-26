using Newtonsoft.Json;
using Ocelot.Configuration;
using Ocelot.Configuration.File;
using Ocelot.DependencyInjection;

namespace Infrastructure.Services
{
    internal enum Server
    {
        AuthenticationService,
        CMSService
    }

    internal class MappedServerAddress
    {
        public string Address { get; set; } = "localhost";
        public int Port { get; set; } = 8080;
    }

    internal class MappedService
    {
        public List<MappedServerAddress> Servers { get; set; } = new List<MappedServerAddress>();
        public string? Scheme { get; set; } = string.Empty;
        public string? AuthorizationKey { get; set; }

    }
    
    internal class Service: MappedService
    {
        public Server ServiceName { get; set; }
    }

    internal class OcelotRoute
    {
        public Service Service { get; set; } = new Service();
        public List<FileRoute> Routes { get; set; } = new List<FileRoute>();
    }

    public class OcelotConfiguration
    {
        private List<OcelotRoute> Routes = new List<OcelotRoute>()
        {
            new OcelotRoute
            {
                Service = new Service()
                {
                    ServiceName = Server.AuthenticationService
                },
                Routes = new List<FileRoute>()
                {
                    new FileRoute()
                    {
                        DownstreamPathTemplate = "/csrf{everything}",
                        UpstreamPathTemplate = "/csrf{everything}",
                        UpstreamHttpMethod = new List<string>() { "Get" },
                    },

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
                        DownstreamPathTemplate = "/api/docs",
                        UpstreamPathTemplate = "/auth/docs",
                        UpstreamHttpMethod = new List<string>() { "Get" },
                    },
                    new FileRoute()
                    {
                        DownstreamPathTemplate = "/api/auth/v1/{everything}",
                        UpstreamPathTemplate = "/auth/{everything}",
                        UpstreamHttpMethod = new List<string>() { "Get", "Post", "Put", "Delete" },
                    },
                    new FileRoute()
                    {
                        DownstreamPathTemplate = "/github/graphql/v1{everything}",
                        UpstreamPathTemplate = "/github/graphql/v1{everything}",
                        UpstreamHttpMethod = new List<string>() { "Get", "Post", "Put", "Delete" },
                    },

                }
            },

            new OcelotRoute()
            {
                Service = new Service()
                {
                    ServiceName = Server.CMSService
                },
                Routes = new List<FileRoute>()
                {
                    new FileRoute()
                    {
                        DownstreamPathTemplate = "/wp-json/wp/v2/posts{everything}",
                        UpstreamPathTemplate = "/cms/posts{everything}",
                        UpstreamHttpMethod = new List<string>() { "Get" },
                    },
                    new FileRoute()
                    {
                        DownstreamPathTemplate = "/wp-json/wp/v2/categories{everything}",
                        UpstreamPathTemplate = "/cms/categories{everything}",
                        UpstreamHttpMethod = new List<string>() { "Get" },
                    },
                    new FileRoute()
                    {
                        DownstreamPathTemplate = "/wp-json/wp/v2/tags{everything}",
                        UpstreamPathTemplate = "/cms/tags{everything}",
                        UpstreamHttpMethod = new List<string>() { "Get" },
                    },
                    new FileRoute()
                    {
                        DownstreamPathTemplate = "/wp-json/wp/v2/users{everything}",
                        UpstreamPathTemplate = "/cms/users{everything}",
                        UpstreamHttpMethod = new List<string>() { "Get" },
                    },
                    new FileRoute()
                    {
                        DownstreamPathTemplate = "/wp-json/wp/v2/media{everything}",
                        UpstreamPathTemplate = "/cms/media{everything}",
                        UpstreamHttpMethod = new List<string>() { "Get" },
                    },
                    new FileRoute()
                    {
                        DownstreamPathTemplate = "/wp-json-openapi{everything}",
                        UpstreamPathTemplate = "/cms/json-openapi{everything}",
                        UpstreamHttpMethod = new List<string>() { "Get" },
                    },
                }
            }


        };

        List<FileRoute> GenerateRoutes(IDictionary<string, MappedService> serverAddressMap)
        {
            var routes = new List<FileRoute>();
            foreach (var route in Routes)
            {
                if (route.Routes == null || route.Routes.Count == 0) continue;
                var serviceName = route.Service.ServiceName.ToString();
                if (!serverAddressMap.ContainsKey(serviceName)) continue;
                if (serverAddressMap[serviceName].Servers == null || serverAddressMap[serviceName].Servers.Count == 0) continue;

                route.Service.Servers = serverAddressMap[serviceName].Servers;
                route.Service.AuthorizationKey = serverAddressMap[serviceName].AuthorizationKey;
                if (!string.IsNullOrEmpty(serverAddressMap[serviceName].Scheme)) route.Service.Scheme = serverAddressMap[serviceName].Scheme;

                foreach(var path in route.Routes)
                {
                    path.DownstreamHostAndPorts = route.Service.Servers.Select(x => new FileHostAndPort(x.Address, x.Port)).ToList();
                    path.DownstreamScheme = route.Service.Scheme;

                    if (path.UpstreamHttpMethod == null || path.UpstreamHttpMethod.Count == 0)
                        path.UpstreamHttpMethod = new List<string>() { "Get", "Post", "Put", "Delete", "Patch", "Options" };

                    if (path.LoadBalancerOptions == null)
                        path.LoadBalancerOptions = new FileLoadBalancerOptions()
                        {
                            Type = "RoundRobin"
                        };

                    routes.Add(path);
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
            
            var gatewayServices = new Dictionary<string, MappedService>();
            
            if(appSettings.ContainsKey("GATEWAY_SERVICES"))
            {
                var gatewayServicesJson = appSettings["GATEWAY_SERVICES"]?.ToString() ?? "{}";
                gatewayServices = JsonConvert.DeserializeObject<Dictionary<string, MappedService>>(gatewayServicesJson) ?? new Dictionary<string, MappedService>();
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