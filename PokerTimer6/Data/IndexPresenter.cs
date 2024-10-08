using PokerTimer6.Models;

namespace PokerTimer6.Data
{
    public class IndexPresenter
    {
        private ICountryDataBroker dataBroker;
        public IndexPresenter(ICountryDataBroker countryService) => this.dataBroker = countryService;
        public string? TypeAheadText;
        public IEnumerable<Country> filteredCountries { get; private set; } = Enumerable.Empty<Country>();
        public async Task<IEnumerable<string>> GetItems(string search)
        {
            var list = await dataBroker.FilteredCountries(search, null);
            return list.Select(item => item.Name).AsEnumerable();
        }
    }

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
