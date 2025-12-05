
using System.Data;

namespace PokerLibrary
{
    public interface IDataAccess
    {
        string ConnectionStringName { get; set; }

        Task<List<T>> LoadData<T>(string sql, object parameters);
        Task<List<T>> LoadData<T, U>(string sql, U parameters);
        Task SaveData<T>(string sql, T parameters);
        Task SaveData<T>(string sql, List<T> parameters);
        Task<int> ExecuteAsync(string sql, object parameters, IDbTransaction? transaction);
        Task ExecuteInTransactionAsync(Func<IDbConnection, IDbTransaction, Task> work);
    }
}