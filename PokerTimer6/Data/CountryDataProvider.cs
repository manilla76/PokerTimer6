namespace PokerTimer6.Data
{
    public sealed class CountryDataProvider : ICountryDataProvider
    {
        private readonly HttpClient httpClient;
        private List<CountryData> baseDataSet = new List<CountryData>();
        public Task LoadTask { get; private set; } = Task.CompletedTask;
        private List<Continent> continents = new List<Continent>();
        private List<Country> countries = new List<Country>();

        public CountryDataProvider(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.LoadTask = LoadBaseData();
        }

        public async ValueTask<IEnumerable<Country>> GetCountriesAsync()
        {
            await this.LoadTask;
            return countries.AsEnumerable();
        }

        public async ValueTask<IEnumerable<Continent>> GetContinentsAsync()
        {
            await this.LoadTask;
            return continents.AsEnumerable();
        }

        public async ValueTask<IEnumerable<Country>> FilteredCountries(string? searchText, Guid? continentUid = null)
            => await this.GetFilteredCountries(searchText, continentUid);

        public async ValueTask<IEnumerable<Country>> FilteredCountriesAsync(Guid continentUid)
        {
            await this.LoadTask;
            return countries.Where(item => item.ContinentUid == continentUid);
        }

        private async ValueTask<IEnumerable<Country>> GetFilteredCountries(string? searchText, Guid? continentUid = null)
        {
            await this.LoadTask;
            var query = countries.AsEnumerable();

            if (continentUid is not null && continentUid != Guid.Empty)
            {
                query = query.Where(item => item.ContinentUid == continentUid);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(item => item.Name.ToLower().Contains(searchText.ToLower()));
            }
            return query.OrderBy(item => item.Name);
        }

        private async Task LoadBaseData()
        {
            baseDataSet =
                await httpClient.GetFromJsonAsync<List<CountryData>>("sample-data/countries.json")
                ?? new List<CountryData>();
            var distinctContinentNames = baseDataSet.Select(item => item.Continent).Distinct().ToList();

            foreach (var continent in distinctContinentNames)
            {
                continents.Add(new Continent { Name = continent });
            }

            foreach (var continent in continents)
            {
                var countryNamesInContinent =
                    baseDataSet
                    .Where(item => item.Continent == continent.Name)
                    .Select(item => item.Country)
                    .ToList();
                foreach (var countryName in countryNamesInContinent)
                {
                    countries.Add(new Country { Name = countryName, ContinentUid = continent.Uid });
                }
            }
        }

        private record CountryData
        {
            public required string Country { get; init; }
            public required string Continent { get; init; }
        }
    }
}
