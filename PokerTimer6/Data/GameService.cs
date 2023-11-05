using PokerLibrary;
using PokerTimer6.Models;
using System;

namespace PokerTimer6.Data
{
    public class GameService : IGameService
    {
        public Queue<Round> Rounds { get; set; } = new Queue<Round>();
        public Round? CurrentRound { get; set; }

        private readonly IPlayerService playerService;
        private readonly IPayoutService payoutService;

        public event Action? OnChange;

        private void NotifyDataChanged() => OnChange?.Invoke();
        public GameService(IPlayerService playerService, IPayoutService payoutService)
        {
            this.playerService = playerService;
            this.payoutService = payoutService;
        }

        public void SetCurrentRound()
        {
            CurrentRound = Rounds.Dequeue();
            NotifyDataChanged();
        }

        public void ResetTournament()
        {
            playerService.ResetPlayers();
            payoutService.ResetPayout();
            NotifyDataChanged();
        }
        public void SetActivePayout()
        {
            payoutService.SetActivePayout(playerService.StartingNumberOfPlayers);
        }

        public void ShufflePlayers()
        {
            playerService.ShufflePlayers();
            payoutService.SetActivePayout(playerService.StartingNumberOfPlayers);
        }

        public void AddSeat()
        {
            playerService.SetSeat();
            payoutService.AddPrizeMoney();
            payoutService.SetActivePayout(playerService.StartingNumberOfPlayers);
        }

    }


}
