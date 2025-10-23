using System.Data;

namespace Package.Database.Dapper.Interfaces
{
    public interface IDbWrapper
    {
        IDbExecutor CreateDbConnection(IDbConnection connection);
    }
}
