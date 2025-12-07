using PokerTimer6.Models;

namespace PokerTimer6.Data.Interfaces
{
    public interface IPlayerService
    {
        List<Player> ActiveList { get; set; }
        List<int> Dealers { get; set; }
        uint NextPlayerId { get; }
        List<Player> Players { get; set; }
        uint StartingNumberOfPlayers { get; }

        event Func<Task>? OnChange;

        void AddPlayer(Player player);
        uint GetNextPlayerId();
        void SetDealer(int table);
        void SetSeat();
        void Shuffle<T>(IList<T> list);
        void ShufflePlayers();
        void ResetPlayers();
        void RemovePlayer(Player player);
        Task SetNextPlayerID(IGameService game);
        Task SavePlayers();
        Task LoadPlayers();
        Task<List<Player>> GetPlayerNames();
        Task<uint> GetNextID();
        Task UpdatePlayersAsync(Player player);
    }
}