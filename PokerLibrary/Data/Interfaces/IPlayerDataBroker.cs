using PokerLibrary.Models;

namespace PokerLibrary.Data.Interfaces
{
    public interface IPlayerDataBroker
    {
        public ValueTask<IEnumerable<Player>> GetPlayersAsync();
        public ValueTask<IEnumerable<Player>> FilteredPlayers(string? searchText, uint? id = null);
        public ValueTask<IEnumerable<Player>> FilteredPlayersAsync(uint id);
    }
}