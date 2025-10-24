using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Text;

namespace AuthenticationService.Infrastructure.Services
{
    public static class JWTAUthenticationServiceProvider
    {
        public static void RegisterJWTToken(this IServiceCollection service, string issuer, string key)
        {
            service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.ClaimsIssuer = issuer;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }
                    };
                });
        }


        public static void RegisterIdentityServer(this IServiceCollection services)
        {
           var authority = "http://localhost:8000/openid";
            var clientId = "579588";
            var clientSecret = "fd156d29095eeb9a12a154d76e52b45f463389d7dbcc9a568b11b07d";
            var redirectUri = "http://localhost:8005/admin/";

            services.AddAuthentication(options =>
    {
        options.DefaultScheme = "Cookies";
        options.DefaultChallengeScheme = "oidc";
    })
            .AddCookie("Cookies");
            // .AddOpenIdConnect("oidc", options =>
            // {
            //     options.Authority = authority;
            //     options.ClientId = clientId;
            //     options.ClientSecret = clientSecret;
            //     options.ResponseType = OpenIdConnectResponseType.Code;
            //     options.SaveTokens = true;
            //     options.RequireHttpsMetadata = false; // dev only
            //     options.GetClaimsFromUserInfoEndpoint = true;
            //     options.Scope.Add("openid");
            //     options.Scope.Add("profile");
            //     options.CallbackPath = redirectUri; // or new PathString("/signin-oidc")
            // });
        }

        public static void RegisterServices(this IServiceCollection service, string issuer, string key)
        {
            service.RegisterJWTToken(issuer, key);
            // service.RegisterIdentityServer();
        }
    }
}
