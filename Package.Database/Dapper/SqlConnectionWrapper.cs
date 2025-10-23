using Microsoft.Data.SqlClient;
using Package.Database.Dapper.Interfaces;
using System.Data;

namespace Package.Database.Dapper
{
    public class SqlConnectionWrapper : ISqlConnectionWrapper
    {
        public IDbConnection CreateSqlConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException("Please provide a valid connection string");
            return new SqlConnection(connectionString);
        }
    }
}
