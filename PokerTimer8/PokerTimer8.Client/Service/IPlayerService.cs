using PokerEntities;

namespace PokerTimer8.Client.Service
{
    public interface IPlayerService
    {
        Task<IEnumerable<Player>?> GetPlayers();
        Task<Player?> GetPlayer(int playerId);
        Task<Player?> GetPlayer(string playerName);
        Task AddPlayer(Player player);
        Task AddPlayer(string playerName);
        Task ModifyPlayer(Player player);
        Task RemovePlayer(Player player);
        Task RemovePlayer(int  playerId);
    }
}
