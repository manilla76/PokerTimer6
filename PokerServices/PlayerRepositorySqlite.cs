using Dapper;
using System.Data.SQLite;
using Microsoft.Extensions.Configuration;
using PokerEntities;
using PokerServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServices
{
    public class PlayerRepositorySqlite : IPlayerRepository
    {
        public Task AddAsync(Player player)
        {
            string sql = @"insert into [Players] (""Name"", ""TableId"", ""Seat"", ""IsActive"") values (@Name, @TableId, @Seat, @IsActive);";
            return DataAccess.SaveData(sql, player);
        }

        public Task AddAsync(string playerName)
        {
            string sql = @"insert into [Players] (""Name"") values (@playerName);";
            return DataAccess.SaveData<dynamic>(sql, new { playerName });
        }

        //public async Task<Player?> GetPlayerAsync(int playerId)
        //{
        //    string sql = @"select * from Players where ""PlayerId"" = @playerId";
        //    var temp = DataAccess.LoadData<Player, int>(sql, playerId);
        //    var result = await DataAccess.LoadData<Player, dynamic>(sql, playerId);
        //    return result.First();
        //}
        //public async Task<List<T>> LoadData<T, U>(string sql, U parameters)
        //{
        //    string connectionString = config.GetConnectionString(ConnectionStringName);
        //    using (IDbConnection connection = new SQLiteConnection(connectionString))
        //    {
        //        var data = await connection.QueryAsync<T>(sql, parameters);
        //        return data.ToList();
        //    }
        //}

        public async Task<Player?> GetPlayerAsync(int playerId)
        {
            string sql = @"select * from Players where PlayerId = @playerId";
            var temp = await DataAccess.LoadData<Player, dynamic>(sql, new {playerId});
            return temp.FirstOrDefault();
        }

        public Task<Player?> GetPlayerAsync(string playerName)
        {
            string sql = @"select * from Players where Name = @playerName";
            var result = DataAccess.LoadData<Player, dynamic>(sql, new { playerName });
            return Task.FromResult(result.Result.First())!;
        }

        public async Task<IEnumerable<Player>?> GetPlayersAsync()
        {
            string sql = "select * from Players";
            var result = await DataAccess.LoadData<Player, dynamic>(sql, new { });
            return result;
        }

        public async Task RemovePlayerAsync(Player player)
        {
            string sql = "delete from Players where PlayerId = @PlayerId";
            await DataAccess.SaveData(sql, player );
        }

        public async Task RemovePlayerAsync(int playerId)
        {
            string sql = @"delete from Players where PlayerId = @playerId";
            await DataAccess.RemoveData<dynamic>(sql, new { playerId });
        }

        public async Task RemovePlayerAsync(string playerName)
        {
            string sql = $"delete from Players where Name = @playerName";
            await DataAccess.RemoveData(sql, new { playerName });
        }

        public async Task UpdatePlayerAsync(Player player)
        {
            string sql = $"update Players set Name = @Name, TableId = @TableId, Seat = @Seat, IsActive = @IsActive where PlayerId = @PlayerId";
            await DataAccess.UpdateData(sql, player);
        }
    }
}
