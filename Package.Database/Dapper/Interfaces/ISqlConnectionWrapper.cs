using System.Data;

namespace Package.Database.Dapper.Interfaces
{
    public interface ISqlConnectionWrapper
    {
        IDbConnection CreateSqlConnection(string connectionString);
    }
}
