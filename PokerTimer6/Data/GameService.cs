using Dapper;
using PokerLibrary;
using PokerLibrary.Models;
using PokerTimer6.Models;
using System;
using System.Runtime.Intrinsics.Arm;

namespace PokerTimer6.Data
{
    public class GameService : IGameService
    {
        public Queue<Round> Rounds { get; set; } = new Queue<Round>();
        public Round? CurrentRound { get; set; }

        private readonly IPlayerService playerService;
        private readonly IPayoutService payoutService;
        private readonly IDataAccess data;
        private int round = 0;

        public event Action? OnChange;

        private void NotifyDataChanged() => OnChange?.Invoke();
        public GameService(IPlayerService playerService, IPayoutService payoutService, IDataAccess data)
        {
            this.playerService = playerService;
            this.payoutService = payoutService;
            this.data = data;
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
        public void AddRound(Round roundModel)
        {
            round++;
            Rounds.Enqueue(new Round { RoundNumber = round, SmallBlind = roundModel.BigBlind / 2 ?? 0, BigBlind = roundModel.BigBlind, RoundTime = new TimeSpan(0, roundModel.RoundMinutes, 0) });
            roundModel.BigBlind = null;
            NotifyDataChanged();
        }

        public void RemoveRound(Round round)
        {
            Rounds = new Queue<Round>(Rounds.Where(r => r != round));
            NotifyDataChanged();
        }
        public async Task SavePlayers()
        {
            await data.SaveData<Player>("insert or ignore into Players (Name) values (@name)", playerService.Players);
        }

        public async Task LoadPlayers()
        {
            var output = await data.LoadData<Player, DynamicParameters>("select name from Players", new DynamicParameters());
            foreach (var item in output)
            {
                playerService.AddPlayer(item);
            }
        }

        public async Task SaveRoundLayoutAsync()
        {
            await data.SaveData<Round>("insert into Rounds (BigBlind, Time) values (@BigBlind, @Time)", Rounds.ToList());
        }

        public async Task LoadRoundLayoutAsync()
        {
            Rounds.Clear();
            var output = await data.LoadData<Round, DynamicParameters>("select BigBlind, Time from Rounds", new DynamicParameters());
            foreach(var item in output)
            {
                item.RoundMinutes = item.Time;
                AddRound(item);
            }
        }
    }


}
