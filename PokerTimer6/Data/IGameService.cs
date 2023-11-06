using PokerTimer6.Models;

namespace PokerTimer6.Data
{
    public interface IGameService
    {
        Queue<Round> Rounds { get; set; }
        Round? CurrentRound { get; set; }

        event Action? OnChange;

        void AddRound(Round roundModel);
        void AddSeat();
        void RemoveRound(Round round);
        void ResetTournament();
        void SetActivePayout();
        void SetCurrentRound();
        void ShufflePlayers();
    }
}