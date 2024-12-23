using System.Data;
using Dapper;
using System.Data.SQLite;
using Microsoft.Extensions.Configuration;

namespace PokerLibrary
{
    public class DataAccess : IDataAccess
    {
        private readonly IConfiguration config;

        public string ConnectionStringName { get; set; } = "Default";

        public DataAccess(IConfiguration config)
        {
            this.config = config;
        }

        /// <summary>
        /// Uses Dapper to get Data from database. Pass in SQL command with parameters.
        /// </summary>
        /// <typeparam name="T">Output Model datatype</typeparam>
        /// <typeparam name="U">Parameter list model type</typeparam>
        /// <param name="sql">SQL command: select * from [tablename] where [column] = @column</param>
        /// <param name="parameters">Parameter list: List of type <typeparamref name="U"/> with parameters for the query</param>
        /// <returns>List of data of type <typeparamref name="T"/></returns>
        public async Task<List<T>> LoadData<T, U>(string sql, U parameters)
        {
            using IDbConnection connection = new SQLiteConnection(GetConnectionString());
            var data = await connection.QueryAsync<T>(sql, parameters);
            return data.ToList();
        }

        /// <summary>
        /// Uses Dapper to send Data to database. Pass in SQL command with parameters.
        /// </summary>
        /// <typeparam name="T">Data Model of input parameter</typeparam>
        /// <param name="sql">SQL command: insert into [table] (col1, col2) values (@prop1, @prop2)</param>
        /// <param name="parameters">Object of type <typeparamref name="T"/> with properties to pass to database.</param>
        /// <returns>Task representing the asynchronous operation</returns>
        public async Task SaveData<T>(string sql, T parameters)
        {
            using IDbConnection connection = new SQLiteConnection(GetConnectionString());
            await connection.ExecuteAsync(sql, parameters);
        }

        /// <summary>
        /// Uses Dapper to send Data to database. Pass in SQL command with parameters.
        /// </summary>
        /// <typeparam name="T">Data Model of input parameters</typeparam>
        /// <param name="sql">SQL command: insert into [table] (col1, col2) values (@prop1, @prop2)</param>
        /// <param name="parameters">List of type <typeparamref name="T"/> with properties to pass to database.</param>
        /// <returns>Task representing the asynchronous operation</returns>
        public async Task SaveData<T>(string sql, List<T> parameters)
        {
            using IDbConnection connection = new SQLiteConnection(GetConnectionString());
            await connection.ExecuteAsync(sql, parameters);
        }

        /// <summary>
        /// Gets the connection string from the configuration.
        /// </summary>
        /// <returns>The connection string</returns>
        private string GetConnectionString()
        {
            return config.GetConnectionString(ConnectionStringName);
        }
    }
}
