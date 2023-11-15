using PokerTimer6.Data.Interfaces;

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
}
