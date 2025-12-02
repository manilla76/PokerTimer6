using PokerTimer6.Data.Interfaces;
using PokerTimer6.Models;

namespace PokerTimer6.Data
{
    public class IndexPresenterPlayer
    {
        private IPlayerDataBroker dataBroker;
        public IndexPresenterPlayer(IPlayerDataBroker playerService) => this.dataBroker = playerService;
        public string? TypeAheadText;
        public IEnumerable<Player> filteredPlayers { get; private set; } = Enumerable.Empty<Player>();
        public async Task<IEnumerable<string>> GetItems(string search)
        {
            var list = await dataBroker.FilteredPlayers(search, null);
            return list.Select(item => item.Name).AsEnumerable();
        }
    }
}
