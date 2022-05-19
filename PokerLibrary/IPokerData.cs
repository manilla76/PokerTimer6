using PokerLibrary.Models;

namespace PokerLibrary
{
    public interface IPokerData
    {
        Task<List<PlayerModel>> GetPlayers();
        Task InsertPlayer(PlayerModel player);
    }
}