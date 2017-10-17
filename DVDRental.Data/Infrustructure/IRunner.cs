using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;

namespace DVDRental.Data.Infrustructure
{
    public interface IRunner
    {
        string ConnectionString { get; set; }
        List<object> ExecuteDynamic(string sql, params object[] args);
        dynamic ExecuteSingleDynamic(string sql, params object[] args);
        IEnumerable<T> Execute<T>(string sql,params object[] args) where T : new();
        T ExecuteSingle<T>(string sql,params object[] args) where T : new();
        Task<IDataReader> OpenReaderAsync(string sqlQuery, params object[] args);
        NpgsqlDataReader OpenReader(string sqlQuery, params object[] args);
        NpgsqlCommand BuildCommand(string sql, params object[] args);
        Task<List<int>> TransactAsync(params NpgsqlCommand[] commands);
        List<int> Transact(params NpgsqlCommand[] commands);
    }
}