namespace PokerTimer6.Data
{
    public sealed class CountryDataBroker : ICountryDataBroker
    {
        private ICountryDataProvider countryDataProvider;
        public CountryDataBroker(ICountryDataProvider countryDataProvider) => this.countryDataProvider = countryDataProvider;
        public async ValueTask<IEnumerable<Country>> GetCountriesAsync() => await countryDataProvider.GetCountriesAsync();
        public async ValueTask<IEnumerable<Continent>> GetContinentsAsync() => await countryDataProvider.GetContinentsAsync();
        public async ValueTask<IEnumerable<Country>> FilteredCountries(string? searchText, Guid? continentUid = null)
            => await countryDataProvider.FilteredCountries(searchText, continentUid);
        public async ValueTask<IEnumerable<Country>> FilteredCountriesAsync(Guid continentUid)
            => await countryDataProvider.FilteredCountriesAsync(continentUid);
    }
}
