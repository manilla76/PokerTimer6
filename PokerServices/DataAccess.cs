using System.Data.SQLite;
using System.Data;
using Dapper;

namespace PokerServices
{
    public static class DataAccess
    {

        private static readonly string connectionString = "Data Source=./pokerDb.db;Mode=ReadWrite";
       
        /// <summary>
        /// Uses Dapper to get Data from database.  Pass in SQL command with parameters.
        /// </summary>
        /// <typeparam name="T">Output Model datatype</typeparam>
        /// <typeparam name="U">Parameter list model type</typeparam>
        /// <param name="sql">SQL command: select * from [tablename] where [column] = @column</param>
        /// <param name="parameters">Parameter list: List of type <typeparamref name="U"/> with paremters for the query</param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> LoadData<T, U>(string sql, U parameters)
        {
            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                var data = await connection.QueryAsync<T>(sql, parameters);
                return data;
            }
        }

        public static async Task RemoveData<T>(string sql, T parameters)
        {
            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                var data = await connection.ExecuteAsync(sql, parameters);
            }
        }

        public static async Task UpdateData<T>(string sql, T parameters)
        {
            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                var data = await connection.ExecuteAsync(sql, parameters);
            }
        }

        /// <summary>
        /// Uses Dapper to send Data to database.  Pass in SQL command with parameters
        /// </summary>
        /// <typeparam name="T">Data Model of input parameter</typeparam>
        /// <param name="sql">SQL command: insert into [table] (col1, col2) values (@prop1, @prop2)</param>
        /// <param name="parameters">object of type <typeparamref name="T"/> with properties to pass to database.</param>
        /// <returns></returns>
        public static async Task SaveData<T>(string sql, T parameters)
        {
            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                var data = await connection.ExecuteAsync(sql, parameters);
            }
        }

        /// <summary>
        /// Uses Dapper to send Data to database.  Pass in SQL command with parameters
        /// </summary>
        /// <typeparam name="T">Data Model of input parameters</typeparam>
        /// <param name="sql">SQL command: insert into [table] (col1, col2) values (@prop1, @prop2)</param>
        /// <param name="parameters">List of type <typeparamref name="T"/> with properties to pass to database.</param>
        /// <returns></returns>
        public static async Task SaveData<T>(string sql, List<T> parameters)
        {
            using (IDbConnection connection = new SQLiteConnection(connectionString))
            {
                var data = await connection.ExecuteAsync(sql, parameters);
            }
        }
    }
}
