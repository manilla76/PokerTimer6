using PokerLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerLibrary
{
    public class PokerData : IPokerData
    {
        private readonly IDataAccess db;

        public PokerData(IDataAccess db)
        {
            this.db = db;
        }

        public Task<List<PlayerModel>> GetPlayers()
        {
            string sql = "select * from Players";
            return db.LoadData<PlayerModel, dynamic>(sql, new { });
        }

        public Task InsertPlayer(PlayerModel player)
        {
            string sql = @"insert into [Players] (""Name"", ""Table"", ""Seat"") values (@Name, @Table, @Seat);";
            return db.SaveData(sql, player);
        }
    }
}
