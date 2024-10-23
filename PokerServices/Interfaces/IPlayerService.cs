using PokerEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServices.Interfaces
{
    public interface IPlayerService
    {
        Task AddPlayerAsync(Player player);

        Task AddPlayerAsync(string  playerName);

        Task RemovePlayerAsync(Player player);

        Task RemovePlayerAsync(int playerId);
        
        Task RemovePlayerAsync(string playerName);

        Task<List<Player>?> GetPlayersAsync();

        Task<Player?> GetPlayerByIdAsync(int playerId);
        Task<Player?> GetPlayerByNameAsync(string playerName);
        Task UpdatePlayerAsync(Player player);
    }
}
