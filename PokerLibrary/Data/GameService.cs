using Dapper;
using PokerLibrary.Models;
using PokerLibrary.Data.Interfaces;

namespace PokerLibrary.Data
{
    public class GameService : IGameService
    {
        public Queue<Round> Rounds { get; set; } = new Queue<Round>();
        public Round CurrentRound { get; set; } = new ();
        public int TournamentID { get; set; } = 1;
        public List<int> TournamentList { get; set; } = new List<int>();
        public event Action? OnChange;
        private void NotifyDataChanged() => OnChange?.Invoke();

        private readonly IPlayerService playerService;
        private readonly IPayoutService payoutService;
        private readonly IDataAccess data;
        private int round = 0;


        public GameService(IPlayerService playerService, IPayoutService payoutService, IDataAccess data)
        {
            this.playerService = playerService;
            this.payoutService = payoutService;
            this.data = data;
        }
       
        /// <summary>
        /// Gets next round from the queue
        /// </summary>
        public void SetCurrentRound()
        {
            CurrentRound = Rounds.Dequeue();
            NotifyDataChanged();
        }
        /// <summary>
        /// Resets the Players and Payouts
        /// </summary>
        public void ResetTournament()
        {
            playerService.ResetPlayers();
            payoutService.ResetPayout();
            NotifyDataChanged();
        }
        /// <summary>
        /// Uses the number of starting players (before the "still playing" box is unchecked) to select the payout structure
        /// </summary>
        public void SetActivePayout()
        {
            payoutService.SetActivePayout(playerService.StartingNumberOfPlayers);
        }
        /// <summary>
        /// Assigns all active players to a table/seat combination and assigns a dealer for each table.
        /// </summary>
        public void ShufflePlayers()
        {
            playerService.ShufflePlayers();
            payoutService.SetActivePayout(playerService.StartingNumberOfPlayers);
        }
        /// <summary>
        /// New player added to active game, assigns seat, adds $ to prize pool, and verifies proper payout structure
        /// </summary>
        public void AddSeat()
        {
            playerService.SetSeat();
            payoutService.AddPrizeMoney();
            payoutService.SetActivePayout(playerService.StartingNumberOfPlayers);
        }
        /// <summary>
        /// Adds a round to the queue
        /// </summary>
        /// <param name="roundModel">Round to add</param>
        public void AddRound(Round roundModel)
        {
            round++;
            Rounds.Enqueue(new Round { id = roundModel.id, Tournament_id = roundModel.Tournament_id, RoundNumber = round, SmallBlind = roundModel.BigBlind / 2 ?? 0, BigBlind = roundModel.BigBlind, RoundTime = new TimeSpan(0, (int)roundModel.RoundMinutes, 0) }); ;
            roundModel.BigBlind = null;
            NotifyDataChanged();
        }
        /// <summary>
        /// Deletes Round from the queue
        /// </summary>
        /// <param name="round">Round to delete</param>
        public void RemoveRound(Round round)
        {
            Rounds = new Queue<Round>(Rounds.Where(r => r != round));
            NotifyDataChanged();
        }
        /// <summary>
        /// Save list of players to the database
        /// </summary>
        /// <returns></returns>
        public async Task SavePlayers()
        {
            await data.SaveData<Player>("insert or ignore into Players (Name) values (@name)", playerService.Players);
        }
        public async Task SavePlayer(Player playerName)
        {
            await data.SaveData<Player>($"insert or ignore into Players (Name) values (@name)", playerName);
        }
        /// <summary>
        /// Loads list of players from the database
        /// </summary>
        /// <returns></returns>
        public async Task LoadPlayers()
        {
            var output = await data.LoadData<Player, DynamicParameters>("select name, id from Players", new DynamicParameters());
            foreach (var item in output)
            {
                playerService.AddPlayer(item);
            }
        }
        /// <summary>
        /// Get list of id's from the players table, return the max + 1
        /// </summary>
        /// <returns></returns>
        public async Task<uint> GetNextID()
        {
            var output = await data.LoadData<int, DynamicParameters>($"select id from players", new DynamicParameters());
            return (uint)output.Max() + 1;
        }
        /// <summary>
        /// Get the available players from the db
        /// </summary>
        /// <returns></returns>
        public async Task<List<Player>> GetPlayerNames()
        {
            var output = await data.LoadData<Player, DynamicParameters>("select name, id from Players", new DynamicParameters());
            return output;
        }
        /// <summary>
        /// Updates selected tournament_id (from UI) blind structure with the current blind structure.
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Creates a new tournament_id in the Round table with the current blind structure.
        /// </summary>
        /// <returns></returns>
        public async Task NewRoundLayoutAsync()
        {

            TournamentList.Add(TournamentList.Max() + 1);
            foreach (var item in Rounds)
            {
                item.Time = (int)item.RoundTime.TotalMinutes;
            }
            
            await data.SaveData<Round>($"insert into Rounds (BigBlind, Time, Tournament_id) values (@BigBlind, @Time, {TournamentList.Max()})", Rounds.ToList());
        }
        /// <summary>
        /// Loads the blind structure of the selected tournament_id (from UI)
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Gets the list of tournament #s from the database (Round table) with blind structures saved.
        /// </summary>
        /// <returns></returns>
        public async Task LoadTournamentListAsync()
        {
            var output = await data.LoadData<int, DynamicParameters>("select distinct Tournament_id from Rounds", new DynamicParameters());
            TournamentList = output.ToList();
        }
    }


}
