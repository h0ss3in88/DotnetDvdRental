using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DVDRental.Data.Infrustructure;
using Npgsql;

namespace DVDRental.Data
{
    public class Runner : IRunner
    {
        public string ConnectionString { get; set; }
        public Runner(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public List<dynamic> ExecuteDynamic(string sql, params object[] args)
        {
            var result = new List<dynamic>();
            var reader = OpenReader(sql, args);
            while (reader.Read())
            {
                result.Add(reader.ToExpando());
            }
            return result;
        }

        public dynamic ExecuteSingleDynamic(string sql, params object[] args)
        {
            return ExecuteDynamic(sql, args).FirstOrDefault();
        }
        public IEnumerable<T> Execute<T>(string sql,params object[] args) where T : new()
        {
            var result = new List<T>();
            var reader = OpenReader(sql, args);        
            while (reader.Read())
            {
                result = reader.ToList<T>();
            }        
            return result;
        }
        public T ExecuteSingle<T>(string sql,params object[] args) where T : new()
        {
            return Execute<T>(sql, args).FirstOrDefault();
        }
        public async Task<IDataReader> OpenReaderAsync(string sqlQuery, params object[] args)
        {
            var connection = new NpgsqlConnection(ConnectionString);
            var command = BuildCommand(sqlQuery, args);
            command.Connection = connection;
            await connection.OpenAsync();
            return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        }
        public NpgsqlDataReader OpenReader(string sqlQuery, params object[] args)
        {
            var connection = new NpgsqlConnection(ConnectionString);
            var command = BuildCommand(sqlQuery, args);
            command.Connection = connection;
            connection.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
        public NpgsqlCommand BuildCommand(string sql, params object[] args)
        {
            var cmd = new NpgsqlCommand {CommandText = sql};
            if (args == null) return cmd;
            foreach (var arg in args)
            {
                cmd.AddParameter(arg);
            }
            return cmd;
        }
        public async Task<List<int>> TransactAsync(params NpgsqlCommand[] commands)
        {
            var result = new List<int>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                using (var trx = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var command in commands)
                        {
                            command.Transaction = trx;
                            command.Connection = connection;
                            result.Add(await command.ExecuteNonQueryAsync());
                        }
                        await trx.CommitAsync();
                    }
                    catch (NpgsqlException e)
                    {
                        await trx.RollbackAsync();
                        throw e;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                return result;
            }
        }
        public List<int> Transact(params NpgsqlCommand[] commands)
        {
            var result = new List<int>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                using (var trx = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var command in commands)
                        {
                            command.Transaction = trx;
                            command.Connection = connection;
                            result.Add(command.ExecuteNonQuery());
                        }
                        trx.Commit();
                    }
                    catch (NpgsqlException e)
                    {
                        trx.Rollback();
                        throw e;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                return result;

            }
        }
       
    }
}