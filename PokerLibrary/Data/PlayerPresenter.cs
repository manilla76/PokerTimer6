using PokerLibrary.Data.Interfaces;
using PokerLibrary.Models;

namespace PokerLibrary.Data
{
    public class PlayerPresenter
    {
        private IPlayerDataBroker playerDataBroker;
        public PlayerPresenter(IPlayerDataBroker playerBroker) => this.playerDataBroker = playerBroker;
        public string? TypeAheadText { get; set; }

        public IEnumerable<Player> filteredPlayerList { get; private set; } = Enumerable.Empty<Player>();
        public async Task<IEnumerable<string>> GetItems(string search)
        {
            var list = await playerDataBroker.FilteredPlayers(search, null);
            return list.Select(item => item.Name).AsEnumerable();
        }
    }
}
