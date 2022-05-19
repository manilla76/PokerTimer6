using System.Data;
using Dapper;
using System.Data.SQLite;
using Microsoft.Extensions.Configuration;

namespace PokerLibrary
{
    public class DataAccess : IDataAccess
    {
        string connectionString;
        private readonly IConfiguration config;

        public string ConnectionStringName { get; set; } = "Default";

        public DataAccess(IConfiguration config)
        {
            this.config = config;
        }

        public async Task<List<T>> LoadData<T, U>(string sql, U parameters)
        {
            string connectionString = config.GetConnectionString(ConnectionStringName);
            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                var data = await connection.QueryAsync<T>(sql, parameters);
                return data.ToList();
            }
        }

        public async Task SaveData<T>(string sql, T parameters)
        {
            string connectionString = config.GetConnectionString(ConnectionStringName);
            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                var data = await connection.ExecuteAsync(sql, parameters);
            }
        }
    }
}
