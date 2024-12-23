using PokerTimer6.Models;
using System.Collections.ObjectModel;

namespace PokerTimer6.Data.Interfaces
{
    public interface IPlayerService
    {
        List<Player> ActiveList { get; set; }
        List<int> Dealers { get; set; }
        uint NextPlayerId { get; }
        uint StartingNumberOfPlayers { get; }
        ObservableCollection<Player> Players { get; set; }

        event Action? OnChange;

        void AddPlayer(Player player);
        uint GetNextPlayerId();
        void SetDealer(int table);
        void SetSeat();
        void Shuffle<T>(IList<T> list);
        void ShufflePlayers();
        void ResetPlayers();
        void RemovePlayer(Player player);
        Task SetNextPlayerID(IGameService game);
    }
}