using Dapper;
using PokerLibrary;
using PokerLibrary.Models;
using PokerTimer6.Data.Interfaces;
using PokerTimer6.Models;
using System;
using System.Runtime.Intrinsics.Arm;

namespace PokerTimer6.Data
{
    /// <summary>
    /// Service for managing game operations such as rounds, players, and payouts.
    /// </summary>
    public class GameService : IGameService
    {
        /// <summary>
        /// Gets or sets the queue of rounds.
        /// </summary>
        public Queue<Round> Rounds { get; set; } = new Queue<Round>();

        /// <summary>
        /// Gets or sets the current round.
        /// </summary>
        public Round CurrentRound { get; set; } = new();

        /// <summary>
        /// Gets or sets the tournament ID.
        /// </summary>
        public int TournamentID { get; set; } = 1;

        /// <summary>
        /// Gets or sets the list of tournament IDs.
        /// </summary>
        public List<int> TournamentList { get; set; } = new List<int>();

        /// <summary>
        /// Event triggered when data changes.
        /// </summary>
        public event Action? OnChange;

        private void NotifyDataChanged() => OnChange?.Invoke();

        private readonly IPlayerService playerService;
        private readonly IPayoutService payoutService;
        private readonly IDataAccess data;
        private int round = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameService"/> class.
        /// </summary>
        /// <param name="playerService">The player service.</param>
        /// <param name="payoutService">The payout service.</param>
        /// <param name="data">The data access service.</param>
        public GameService(IPlayerService playerService, IPayoutService payoutService, IDataAccess data)
        {
            this.playerService = playerService;
            this.payoutService = payoutService;
            this.data = data;
        }

        /// <summary>
        /// Sets the current round from the queue of rounds.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when no rounds are available.</exception>
        public void SetCurrentRound()
        {
            if (Rounds.Count > 0)
            {
                CurrentRound = Rounds.Dequeue();
                NotifyDataChanged();
            }
            else
            {
                throw new InvalidOperationException("No rounds available to set as current round.");
            }
        }

        /// <summary>
        /// Resets the tournament by resetting players and payouts.
        /// </summary>
        public void ResetTournament()
        {
            playerService.ResetPlayers();
            payoutService.ResetPayout();
            NotifyDataChanged();
        }

        /// <summary>
        /// Sets the active payout based on the starting number of players.
        /// </summary>
        public void SetActivePayout()
        {
            payoutService.SetActivePayout(playerService.StartingNumberOfPlayers);
        }

        /// <summary>
        /// Shuffles the players and updates the active payout.
        /// </summary>
        public void ShufflePlayers()
        {
            playerService.ShufflePlayers();
            payoutService.SetActivePayout(playerService.StartingNumberOfPlayers);
        }

        /// <summary>
        /// Adds a seat for a player and updates the payout.
        /// </summary>
        public void AddSeat()
        {
            playerService.SetSeat();
            payoutService.AddPrizeMoney();
            payoutService.SetActivePayout(playerService.StartingNumberOfPlayers);
        }

        /// <summary>
        /// Adds a round to the queue of rounds.
        /// </summary>
        /// <param name="roundModel">The round model to add.</param>
        /// <exception cref="ArgumentException">Thrown when the round model has invalid values.</exception>
        public void AddRound(Round roundModel)
        {
            if (roundModel.RoundMinutes.HasValue)
            {
                round++;
                Rounds.Enqueue(new Round
                {
                    id = roundModel.id,
                    Tournament_id = roundModel.Tournament_id,
                    RoundNumber = round,
                    SmallBlind = roundModel.BigBlind / 2,
                    BigBlind = roundModel.BigBlind,
                    RoundTime = TimeSpan.FromMinutes(roundModel.RoundMinutes.Value)
                });
                NotifyDataChanged();
            }
            else
            {
                throw new ArgumentException("Round model must have valid RoundMinutes values.");
            }
        }

        /// <summary>
        /// Removes a round from the queue of rounds.
        /// </summary>
        /// <param name="round">The round to remove.</param>
        public void RemoveRound(Round round)
        {
            Rounds = new Queue<Round>(Rounds.Where(r => r != round));
            NotifyDataChanged();
        }

        /// <summary>
        /// Saves the players to the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown when saving players fails.</exception>
        public async Task SavePlayers()
        {
            try
            {
                await data.SaveData("insert or ignore into Players (Name) values (@name)", playerService.Players);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new InvalidOperationException("Failed to save players.", ex);
            }
        }

        /// <summary>
        /// Loads the players from the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown when loading players fails.</exception>
        public async Task LoadPlayers()
        {
            try
            {
                var output = await data.LoadData<Player, DynamicParameters>("select name, id from Players", new DynamicParameters());
                foreach (var item in output)
                {
                    playerService.AddPlayer(item);
                }
            }
            catch (Exception ex)
            {
                // Log exception
                throw new InvalidOperationException("Failed to load players.", ex);
            }
        }

        /// <summary>
        /// Gets the next available player ID.
        /// </summary>
        /// <returns>The next available player ID.</returns>
        /// <exception cref="InvalidOperationException">Thrown when getting the next ID fails.</exception>
        public async Task<uint> GetNextID()
        {
            try
            {
                var output = await data.LoadData<int, DynamicParameters>("select id from players", new DynamicParameters());
                return (uint)output.Max() + 1;
            }
            catch (Exception ex)
            {
                // Log exception
                throw new InvalidOperationException("Failed to get next ID.", ex);
            }
        }

        /// <summary>
        /// Gets the list of player names.
        /// </summary>
        /// <returns>A list of players.</returns>
        /// <exception cref="InvalidOperationException">Thrown when getting player names fails.</exception>
        public async Task<List<Player>> GetPlayerNames()
        {
            try
            {
                var output = await data.LoadData<Player, DynamicParameters>("select name, id from Players", new DynamicParameters());
                return output;
            }
            catch (Exception ex)
            {
                // Log exception
                throw new InvalidOperationException("Failed to get player names.", ex);
            }
        }

        /// <summary>
        /// Gets a dictionary of players with their names.
        /// </summary>
        /// <returns>A sorted dictionary of players and their names.</returns>
        /// <exception cref="InvalidOperationException">Thrown when getting the player dictionary fails.</exception>
        public async Task<SortedDictionary<Player, string>> GetPlayerDictionary()
        {
            try
            {
                var list = await GetPlayerNames();
                return new SortedDictionary<Player, string>(list.Distinct().ToDictionary(x => x, x => x.Name));
            }
            catch (Exception ex)
            {
                // Log exception
                throw new InvalidOperationException("Failed to get player dictionary.", ex);
            }
        }

        /// <summary>
        /// Saves the round layout to the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown when saving the round layout fails.</exception>
        public async Task SaveRoundLayoutAsync()
        {
            try
            {
                var IdList = await data.LoadData<Round, DynamicParameters>($"select id from Rounds where Tournament_id = {TournamentID}", new DynamicParameters());
                var updateList = Rounds.Where(r => IdList.Any(i => i.id == r.id)).ToList();
                var insertList = Rounds.Where(r => !IdList.Select(i => i.id).Contains(r.id)).ToList();
                var deleteIntList = IdList.Where(i => !Rounds.Select(r => r.id).Contains(i.id)).ToList();

                await data.SaveData($"update Rounds set BigBlind = @BigBlind, Time = @Time, Tournament_id = {TournamentID} where id = @id", updateList);
                await data.SaveData($"insert into Rounds (BigBlind, Time, Tournament_id) values (@BigBlind, @Time, {TournamentID})", insertList);
                await data.SaveData($"delete from Rounds where id = @id", deleteIntList);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new InvalidOperationException("Failed to save round layout.", ex);
            }
        }

        /// <summary>
        /// Creates a new round layout and saves it to the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown when creating a new round layout fails.</exception>
        public async Task NewRoundLayoutAsync()
        {
            try
            {
                TournamentList.Add(TournamentList.Max() + 1);
                foreach (var item in Rounds)
                {
                    item.Time = (int)item.RoundTime.TotalMinutes;
                }

                await data.SaveData($"insert into Rounds (BigBlind, Time, Tournament_id) values (@BigBlind, @Time, {TournamentList.Max()})", Rounds.ToList());
            }
            catch (Exception ex)
            {
                // Log exception
                throw new InvalidOperationException("Failed to create new round layout.", ex);
            }
        }

        /// <summary>
        /// Loads the round layout from the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown when loading the round layout fails.</exception>
        public async Task LoadRoundLayoutAsync()
        {
            try
            {
                Rounds.Clear();
                var output = await data.LoadData<Round, DynamicParameters>($"select id, BigBlind, Time, Tournament_id from Rounds where Tournament_id = {TournamentID}", new DynamicParameters());
                foreach (var item in output)
                {
                    item.RoundMinutes = item.Time;
                    AddRound(item);
                }
            }
            catch (Exception ex)
            {
                // Log exception details
                Console.WriteLine($"Failed to load round layout: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw new InvalidOperationException("Failed to load round layout.", ex);
            }
        }

        /// <summary>
        /// Loads the list of tournament IDs from the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown when loading the tournament list fails.</exception>
        public async Task LoadTournamentListAsync()
        {
            try
            {
                var output = await data.LoadData<int, DynamicParameters>("select distinct Tournament_id from Rounds", new DynamicParameters());
                TournamentList = output.ToList();
            }
            catch (Exception ex)
            {
                // Log exception
                throw new InvalidOperationException("Failed to load tournament list.", ex);
            }
        }
    }


}
