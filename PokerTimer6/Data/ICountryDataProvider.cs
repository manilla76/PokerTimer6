namespace PokerTimer6.Data
{
    public interface ICountryDataProvider
    {
        Task LoadTask { get; }

        ValueTask<IEnumerable<Country>> FilteredCountries(string? searchText, Guid? continentUid = null);
        ValueTask<IEnumerable<Country>> FilteredCountriesAsync(Guid continentUid);
        ValueTask<IEnumerable<Continent>> GetContinentsAsync();
        ValueTask<IEnumerable<Country>> GetCountriesAsync();
    }
}