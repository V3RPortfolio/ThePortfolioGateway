using Dapper;
using Package.Database.Dapper.Interfaces;
using System.Data;

namespace Package.Database.Dapper
{
    public class DbExecutor : Disposable, IDbExecutor
    {
        readonly IDbConnection _dbConnection;

        public DbExecutor(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public void Open()
        {
            // TODO: What about transaction management?
            _dbConnection.Open();
        }

        public void Close()
        {
            _dbConnection.Close();
        }

        public ConnectionState State
        {
            get { return _dbConnection.State; }
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _dbConnection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Did not unit test this feature as Dapper Moq does not yet support ExecuteAsync extension
        /// method
        /// </summary>
        public Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _dbConnection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// Did not unit test this feature as Dapper Moq does not yet support ExecuteScalarAsync
        ///extension method
        /// </summary>
        public Task<object> ExecuteScalarAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return _dbConnection.ExecuteScalarAsync(sql, param, transaction, commandTimeout, commandType);
        }

        protected override void DisposeManagedResources()
        {
            _dbConnection.Dispose();
        }
    }
}
