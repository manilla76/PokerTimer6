using PokerLibrary.Models;

namespace PokerLibrary
{
    public interface IPokerData
    {
        Task<SortedDictionary<PlayerModel, string>> GetPlayerDictionary();
        Task<List<PlayerModel>> GetPlayers();
        Task InsertPlayer(PlayerModel player);
    }
}