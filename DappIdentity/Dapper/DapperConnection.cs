using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace DappIdentity.Dapper
{
    public class DapperConnection : IDisposable, IDapperConnection
    {
        public bool Disposed { get; private set; }
        public IDbConnection Socket { get; }
        public DapperConnection(string connectionString)
        {
            Disposed = false;
            Socket = new SqlConnection(connectionString);
       }

        public async Task<T> FirstOrDefault<T>(string query, object param = null) where T : class
        {
            Socket.Open();
            var result = await Socket.QueryFirstOrDefaultAsync<T>(query, param);
            Socket.Close();
            return result;
        }

        public IEnumerable<T> ToEnumerable<T>(string query, object param = null) where T : class
        {
            Socket.Open();
            var result = Socket.Query<T>(query, param);
            Socket.Close();
            return result;
        }

        public async Task Execute(string command, object param = null)
        {
            Socket.Open();
            await Socket.ExecuteAsync(command, param);
            Socket.Close();
        }

        public void Dispose()
        {
            if (Socket.State != ConnectionState.Closed)
                Socket.Close();
            Socket.Dispose();
            Disposed = true;
        }
    }
}