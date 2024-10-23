using PokerEntities;
using PokerServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServices
{
    public class PlayerService(IPlayerRepository playerRepository) : IPlayerService
    {
        private readonly IPlayerRepository playerRepository = playerRepository;

        public async Task AddPlayerAsync(Player player) => await playerRepository.AddAsync(player);

        public async Task AddPlayerAsync(string playerName) => await playerRepository.AddAsync(playerName);

        public Task<Player?> GetPlayerByIdAsync(int playerId) => playerRepository.GetPlayerAsync(playerId);

        public Task<Player?> GetPlayerByNameAsync(string playerName) => playerRepository.GetPlayerAsync(playerName);

        public async Task<List<Player>?> GetPlayersAsync() => (await playerRepository.GetPlayersAsync())?.ToList() ?? default;

        public async Task RemovePlayerAsync(Player player) => await playerRepository.RemovePlayerAsync(player);

        public async Task RemovePlayerAsync(int playerId) => await playerRepository.RemovePlayerAsync(playerId);

        public async Task RemovePlayerAsync(string playerName) => await playerRepository.RemovePlayerAsync(playerName);

        public async Task UpdatePlayerAsync(Player player) => await playerRepository.UpdatePlayerAsync(player);
    }
}
