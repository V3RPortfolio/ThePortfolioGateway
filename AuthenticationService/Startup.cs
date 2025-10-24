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

        private string GetDBConnectionString()
        {
            // "Data Source=host.docker.internal;Initial Catalog=portfolio_auth;User ID=sa;Password=Admin@123;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True;Authentication=SqlPassword"
            var section = Configuration.GetSection("DATABASE");

            var dbHost = section.GetValue<string>("HOST");
            if (dbHost == null) dbHost = "127.0.0.1";

            var dbPassword = section.GetValue<string>("PASSWORD");
            var dbUsername = section.GetValue<string>("SERNAME");
            var encrypt = section.GetValue<string>("ENCRYPT");
            if(encrypt == null) encrypt = "False";
            var trustServerCertificate = Environment.GetEnvironmentVariable("TRUSTSERVERCERTIFICATE");
            if(trustServerCertificate == null) trustServerCertificate = "True";

            var databaseName = Configuration.GetSection("ConnectionStrings").GetValue<string>("Database");
            if (databaseName == null) databaseName = "portfolio_auth";

            var persistSecurityInfo = Configuration.GetSection("ConnectionStrings").GetValue<string>("PersistSecurityInfo");
            if (persistSecurityInfo == null) persistSecurityInfo = "False";

            var pooling = Configuration.GetSection("ConnectionStrings").GetValue<string>("Pooling");
            if (pooling == null) pooling = "False";

            var multipleActiveResultSets = Configuration.GetSection("ConnectionStrings").GetValue<string>("MultipleActiveResultSets");
            if (multipleActiveResultSets == null) multipleActiveResultSets = "False";

            var connectTimeout = Configuration.GetSection("ConnectionStrings").GetValue<string>("ConnectTimeout");
            if (connectTimeout == null) connectTimeout = "60";

            var connectionString = $"Data Source={dbHost};Initial Catalog={databaseName};Persist Security Info={persistSecurityInfo};Pooling={pooling};MultipleActiveResultSets={multipleActiveResultSets};Connect Timeout={connectTimeout};Encrypt={encrypt};TrustServerCertificate={trustServerCertificate};";
            
            if(dbUsername != null && dbPassword != null)
            {
                connectionString += $"User ID={dbUsername};Password={dbPassword};Authentication=SqlPassword;";
            } else
            {
                connectionString += "Integrated Security=True;";
            }

            return connectionString;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var authConnection = GetDBConnectionString();

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
