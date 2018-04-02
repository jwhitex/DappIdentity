using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DappIdentity.Dapper
{
    public interface IDapperConnection
    {
        bool Disposed { get; }
        IDbConnection Socket { get; }
        void Dispose();
        Task Execute(string command, object param = null);
        Task<T> FirstOrDefault<T>(string query, object param = null) where T : class;
        IEnumerable<T> ToEnumerable<T>(string query, object param = null) where T : class;
    }
}