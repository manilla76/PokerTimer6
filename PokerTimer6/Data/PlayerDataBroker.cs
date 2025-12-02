using PokerTimer6.Data.Interfaces;
using PokerTimer6.Models;

namespace PokerTimer6.Data
{
    public sealed class PlayerDataBroker : IPlayerDataBroker
    {
        private IPlayerDataProvider playerDataProvider;
        public PlayerDataBroker(IPlayerDataProvider playerDataProvider) => this.playerDataProvider = playerDataProvider;
        
        public async ValueTask<IEnumerable<Player>> FilteredPlayers(string? searchText, uint? playerUid)
            => await playerDataProvider.FilteredPlayers(searchText, playerUid);
       

        public async ValueTask<IEnumerable<Player>> FilteredPlayersAsync(uint playerUid)
            => await playerDataProvider.FilteredPlayersAsync(playerUid);


        public async ValueTask<IEnumerable<Player>> GetPlayersAsync() => await playerDataProvider.GetPlayersAsync();
        
    }
}
