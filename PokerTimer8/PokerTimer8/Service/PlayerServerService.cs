using PokerEntities;
using PokerServices.Interfaces;

namespace PokerTimer8.Service
{
    public class PlayerServerService : Client.Service.IPlayerService
    {
        private readonly IPlayerRepository playerRepository;
        public PlayerServerService(IPlayerRepository playerRepository)
        {
            this.playerRepository = playerRepository;
        }

        public Task AddPlayer(Player player)
        {
            throw new NotImplementedException();
        }

        public Task AddPlayer(string playerName)
        {
            throw new NotImplementedException();
        }

        public async Task<Player?> GetPlayer(int playerId)
        {
            return await playerRepository.GetPlayerAsync(playerId);
        }

        public Task<Player?> GetPlayer(string playerName)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Player>?> GetPlayers()
        {
            return await playerRepository.GetPlayersAsync();
        }

        public Task ModifyPlayer(Player player)
        {
            throw new NotImplementedException();
        }

        public async Task RemovePlayer(Player player)
        {
            await playerRepository.RemovePlayerAsync(player);
        }

        public async Task RemovePlayer(int playerId)
        {
            await playerRepository.RemovePlayerAsync(playerId);
        }
    }
}
