using PokerEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServices.Interfaces
{
    public interface IPlayerRepository
    {
        Task AddAsync(Player player);
        Task AddAsync(string playerName);
        Task<Player?> GetPlayerAsync(int playerId);
        Task<Player?> GetPlayerAsync(string playerName);
        Task<IEnumerable<Player>?> GetPlayersAsync();
        Task RemovePlayerAsync(Player player);
        Task RemovePlayerAsync(int playerId);
        Task RemovePlayerAsync(string playerName);
        Task UpdatePlayerAsync(Player player);
    }
}
