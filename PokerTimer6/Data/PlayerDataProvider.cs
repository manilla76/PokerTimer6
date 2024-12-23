using PokerTimer6.Data.Interfaces;
using PokerTimer6.Models;
using System.Collections.ObjectModel;

namespace PokerTimer6.Data
{
    public sealed class PlayerDataProvider : IPlayerDataProvider
    {
        private ObservableCollection<Player> baseDataSet = new ();
        public Task LoadTask { get; private set; } = Task.CompletedTask;
        private ObservableCollection<Player> players = new ();
        private readonly IPlayerService playerService;

        public PlayerDataProvider(IPlayerService playerService)
        {
            this.playerService = playerService;
            LoadTask = LoadBaseData();
        }

        public async ValueTask<IEnumerable<Player>> GetPlayersAsync()
        {
            await LoadTask;
            return playerService.Players.AsEnumerable();
        }

        public async ValueTask<IEnumerable<Player>> FilteredPlayers(string? searchText, uint? playerId = null)
            => await GetFilteredPlayers(searchText, playerId);

        public async ValueTask<IEnumerable<Player>> FilteredPlayersAsync(uint playerId)
        {
            await LoadTask;
            return playerService.Players.Where(item => item.id == playerId);
        }

        private async ValueTask<IEnumerable<Player>> GetFilteredPlayers(string? searchText, uint? playerId)
        {
            await LoadTask;
            var query = playerService.Players.AsEnumerable();
            if (playerId is not null)
            {
                query = query.Where(item => item.id == playerId);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(item => item.Name.ToLower().Contains(searchText.ToLower()));
            }
            return query.OrderBy(item => item.Name);
        }

        private async Task LoadBaseData()
        {
            baseDataSet = playerService.Players;
        }
    }
}
