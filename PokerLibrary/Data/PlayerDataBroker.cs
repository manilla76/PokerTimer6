using PokerLibrary.Data.Interfaces;
using PokerLibrary.Models;

namespace PokerLibrary.Data
{
    public class PlayerDataBroker : IPlayerDataBroker
    {
        private IPlayerDataProvider playerDataProvider;

        public PlayerDataBroker(IPlayerDataProvider playerDataProvider) => this.playerDataProvider = playerDataProvider;

        public async ValueTask<IEnumerable<Player>> FilteredPlayers(string? searchText, uint? id = null) => await playerDataProvider.FilteredPlayers(searchText, id);

        public async ValueTask<IEnumerable<Player>> FilteredPlayersAsync(uint id) => await playerDataProvider.FilteredPlayersAsync(id);

        public async ValueTask<IEnumerable<Player>> GetPlayersAsync() => await playerDataProvider.GetPlayersAsync();

    }
}
