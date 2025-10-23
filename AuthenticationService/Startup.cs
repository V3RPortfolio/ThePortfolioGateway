using AuthenticationService.Infrastructure.Services;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace AuthenticationService
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var authConnection = Configuration.GetConnectionString("Auth");

            var issuer = Configuration.GetSection("jwt").GetValue<string>("issuer");
            var key = Configuration.GetSection("jwt").GetValue<string>("key");

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            JWTAUthenticationServiceProvider.RegisterServices(services, issuer, key);
            services.AddOcelot().AddCacheManager(settings => settings.WithDictionaryHandle());
            services.AddSwaggerGen();

            // DependencyInjection.RegisterServices(services, authConnection);
            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseOcelot().Wait();
        }
    }
}
