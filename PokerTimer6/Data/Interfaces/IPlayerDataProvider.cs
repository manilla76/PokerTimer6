using PokerTimer6.Models;

namespace PokerTimer6.Data.Interfaces
{
    public interface IPlayerDataProvider
    {
        Task LoadTask { get; }

        ValueTask<IEnumerable<Player>> FilteredPlayers(string? searchText, uint? id);
        ValueTask<IEnumerable<Player>> FilteredPlayersAsync(uint id);
        ValueTask<IEnumerable<Player>> GetPlayersAsync();
    }
}