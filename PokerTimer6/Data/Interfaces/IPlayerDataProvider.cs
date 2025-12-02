using PokerTimer6.Models;

namespace PokerTimer6.Data.Interfaces
{
    public interface IPlayerDataProvider
    {
        Task LoadTask { get; }

        ValueTask<IEnumerable<Player>> FilteredPlayersAsync(uint playerId);
        ValueTask<IEnumerable<Player>> FilteredPlayers(string? searchText, uint? playerId = null);
        ValueTask<IEnumerable<Player>> GetPlayersAsync();
    }
}