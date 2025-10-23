using AuthenticationService.Database.Dapper;
using AuthenticationService.Database.EntityFramework.Context;
using AuthenticationService.Domain.Log;
using Microsoft.EntityFrameworkCore;
using Package.Database.EntityFramework;
using Package.Database.EntityFramework.Interfaces;

namespace AuthenticationService.Infrastructure.Services
{
    public static class DependencyInjection
    {
        public static void RegisterServices(this IServiceCollection service, string authConnection)
        {
            // Dependency Injection for Entity Framework
            service.AddSingleton<AuthContext, AuthContext>();
            service.AddDbContext<GenericContext<AuthContext>>(opt => opt.UseSqlServer(authConnection), ServiceLifetime.Scoped);
            service.AddTransient<IRepository<LogDto, AuthContext>, Repository<LogDto, AuthContext>>();

            // Dependency Injection for Dapper
            Package.Database.Dapper.Infrastructure.DependencyInjection.AddInfrastructure(service);
            service.AddSingleton<IRepositoryConfiguration>(new RepositoryConfiguration()
            {
                AuthenticationDatabase = authConnection
            });
        }
    }
}
