using PokerLibrary.Data.Interfaces;
using PokerLibrary.Models;

namespace PokerLibrary.Data
{
    public class PlayerDataProvider : IPlayerDataProvider
    {
        private List<Player> baseDataSet = new List<Player>();
        private List<Player> players = new List<Player>();
        private readonly IGameService gameService;

        public Task LoadTask { get; private set; } = Task.CompletedTask;

        public PlayerDataProvider(IGameService gameService)
        {
            this.gameService = gameService;
            LoadTask = LoadBaseData();
        }

        private async Task LoadBaseData()
        {
            baseDataSet = await gameService.GetPlayerNames();

            players = baseDataSet;  // not sure if 2 are required here.  
        }

        public async ValueTask<IEnumerable<Player>> FilteredPlayers(string? searchText, uint? id) => await GetFilteredPlayers(searchText, id);

        private async ValueTask<IEnumerable<Player>> GetFilteredPlayers(string? searchText, uint? id = null)
        {
            await LoadTask;

            var query = players.AsEnumerable();

            if (id is not null && id.HasValue)
            {
                query = query.Where(item => item.id == id);
            }
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(item => item.Name.ToLower().Contains(searchText.ToLower()));
            }
            return query.OrderBy(item => item.Name);
        }

        public async ValueTask<IEnumerable<Player>> FilteredPlayersAsync(uint id)
        {
            await LoadTask;
            return players.Where(item => item.id == id);
        }

        public async ValueTask<IEnumerable<Player>> GetPlayersAsync()
        {
            await LoadTask;
            return players.AsEnumerable();
        }
    }
}