using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Package.Database.Dapper.Interfaces;
using System.Data;

namespace Package.Database.Dapper.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IDbExecutor, DbExecutor>();
            services.AddTransient<IDbConnection, SqlConnection>();
            services.AddTransient<IDbWrapper, DbWrapper>();
            services.AddTransient<ISqlConnectionWrapper, SqlConnectionWrapper>();
        }
    }
}
