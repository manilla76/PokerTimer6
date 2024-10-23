using PokerEntities;
using PokerServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServices
{
    public class PlayerRepositoryStaticData : IPlayerRepository
    {
        private readonly List<Player> players = new List<Player> {
                new Player { Name = "abc", PlayerId = 1 },
                new Player { Name = "def", PlayerId = 2 },
                new Player { Name = "ghi", PlayerId = 3 },
                new Player { Name = "jkl", PlayerId = 4 },
                new Player { Name = "mno", PlayerId = 5 }
            };

        private int nextPlayerId => players.Max(p => p.PlayerId) + 1; 
        public Task AddAsync(Player player)
        {
            if (players is not null && !players.Contains(player))
            {
                players.Add(player);
            }

            return Task.CompletedTask;
        }

        public Task AddAsync(string playerName)
        {
            if (players is not null && !(players.Any(p => p.Name.Equals(playerName, StringComparison.CurrentCultureIgnoreCase))))
            {
                players.Add(new Player { Name = playerName, PlayerId = nextPlayerId });
            }
            return Task.CompletedTask;
        }

        public Task<Player?> GetPlayerAsync(int playerId) => Task.FromResult(players.FirstOrDefault(p => p.PlayerId == playerId));

        public Task<Player?> GetPlayerAsync(string playerName) => Task.FromResult(players.FirstOrDefault(p => p.Name == playerName));

        public Task<IEnumerable<Player>?> GetPlayersAsync() => Task.FromResult(players.Where(p => p.PlayerId > -1) ?? default);

        public Task RemovePlayerAsync(Player player)
        {
            players.Remove(player);
            return Task.CompletedTask;
        }

        public Task RemovePlayerAsync(int playerId)
        {
            players.RemoveAll(p=>p.PlayerId == playerId);
            return Task.CompletedTask;
        }

        public Task RemovePlayerAsync(string playerName)
        {
            players.RemoveAll(p=>p.Name.Equals(playerName, StringComparison.CurrentCultureIgnoreCase));
            return Task.CompletedTask;
        }

        public Task UpdatePlayerAsync(Player player)
        {
            var origPlayer = players.FirstOrDefault(p=>p.PlayerId == player.PlayerId);
            if (origPlayer is not null)
                origPlayer = player;
            return Task.CompletedTask;
        }
    }
}
