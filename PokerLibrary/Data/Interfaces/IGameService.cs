using PokerLibrary.Models;

namespace PokerLibrary.Data.Interfaces
{
    public interface IGameService
    {
        Queue<Round> Rounds { get; set; }
        Round CurrentRound { get; set; }
        int TournamentID { get; set; }
        List<int> TournamentList { get; set; }

        event Action? OnChange;
        void AddRound(Round roundModel);
        void AddSeat();
        Task<uint> GetNextID();
        Task<List<Player>> GetPlayerNames();
        Task LoadPlayers();
        Task LoadRoundLayoutAsync();
        Task LoadTournamentListAsync();
        Task NewRoundLayoutAsync();
        void RemoveRound(Round round);
        void ResetTournament();
        Task SavePlayers();
        Task SaveRoundLayoutAsync();
        void SetActivePayout();
        void SetCurrentRound();
        void ShufflePlayers();
    }
}