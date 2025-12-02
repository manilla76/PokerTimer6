using PokerTimer6.Models;

namespace PokerTimer6.Data.Interfaces
{
    public interface IPlayerDataBroker
    {
        ValueTask<IEnumerable<Player>> FilteredPlayers(string? searchText, uint? playerUid = null);
        ValueTask<IEnumerable<Player>> FilteredPlayersAsync(uint playerUid);
        ValueTask<IEnumerable<Player>> GetPlayersAsync();
    }
}
