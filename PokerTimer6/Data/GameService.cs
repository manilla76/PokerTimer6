using PokerLibrary;
using PokerTimer6.Models;
using System;

namespace PokerTimer6.Data
{
    public class GameService
    {
        public Queue<Round> Rounds { get; set; } = new Queue<Round>();

        public Round? CurrentRound;
        
        private readonly PlayerService playerService;
        private readonly PayoutService payoutService;

        public event Action? OnChange;

        private void NotifyDataChanged() => OnChange?.Invoke();
        public GameService(PlayerService playerService, PayoutService payoutService)
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
