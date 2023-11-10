using Dapper;
using PokerLibrary;
using PokerLibrary.Models;
using PokerTimer6.Data.Interfaces;
using PokerTimer6.Models;
using System;
using System.Runtime.Intrinsics.Arm;

namespace PokerTimer6.Data
{
    public class GameService : IGameService
    {
        public Queue<Round> Rounds { get; set; } = new Queue<Round>();
        public Round CurrentRound { get; set; } = new ();

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
        public int TournamentID { get; set; } = 1;
        public List<int> TournamentList { get; set; } = new List<int>();

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
            Rounds.Enqueue(new Round { id = roundModel.id, Tournament_id = roundModel.Tournament_id, RoundNumber = round, SmallBlind = roundModel.BigBlind / 2 ?? 0, BigBlind = roundModel.BigBlind, RoundTime = new TimeSpan(0, (int)roundModel.RoundMinutes, 0) }); ;
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
            // get the id#s of this tournament_id from the database
            // update any matching id's
            // delete from the db any id's that no longer exist
            // insert to the db any id's that don't match a current row

            var IdList = await data.LoadData<Round, DynamicParameters>($"select id from Rounds where Tournament_id = {TournamentID}", new DynamicParameters());
            var updateList = (from r in Rounds where IdList.Any(i => i.id == r.id) select r).ToList();// Gets the rounds that need to be updated
            var insertList = Rounds.Where(r => !(IdList.Select(i=> i.id).Contains(r.id))).ToList();  // Gets the rounds to be inserted
            var deleteIntList = IdList.Where(i => !Rounds.Select(r => r.id).Contains(i.id)).ToList();      // Gets the list of id's to be deleted
            await data.SaveData<Round>($"update Rounds set BigBlind = @BigBlind, Time = @Time, Tournament_id = {TournamentID} where id = @id", updateList.ToList());
            await data.SaveData<Round>($"insert into Rounds (BigBlind, Time, Tournament_id) values (@BigBlind, @Time, {TournamentID})", insertList);
            await data.SaveData<Round>($"delete from Rounds where id = @id", deleteIntList);
        }
        public async Task NewRoundLayoutAsync()
        {

            TournamentList.Add(TournamentList.Max() + 1);
            foreach (var item in Rounds)
            {
                item.Time = (int)item.RoundTime.TotalMinutes;
            }
            
            await data.SaveData<Round>($"insert into Rounds (BigBlind, Time, Tournament_id) values (@BigBlind, @Time, {TournamentList.Max()})", Rounds.ToList());
        }

        public async Task LoadRoundLayoutAsync()
        {
            Rounds.Clear();
            var output = await data.LoadData<Round, DynamicParameters>($"select id, BigBlind, Time, Tournament_id from Rounds where Tournament_id = {TournamentID}", new DynamicParameters());
            foreach(var item in output)
            {
                item.RoundMinutes = item.Time;
                AddRound(item);
            }
        }

        public async Task LoadTournamentListAsync()
        {
            var output = await data.LoadData<int, DynamicParameters>("select distinct Tournament_id from Rounds", new DynamicParameters());
            TournamentList = output.ToList();
        }
    }


}
