using Package.Database.Dapper.Interfaces;
using System.Data;

namespace Package.Database.Dapper
{
    public class DbWrapper : IDbWrapper
    {
        public IDbExecutor CreateDbConnection(IDbConnection connection)
        {
            if (connection == null) throw new ArgumentException("A connection must be provided");
            return new DbExecutor(connection);
        }
    }
}
