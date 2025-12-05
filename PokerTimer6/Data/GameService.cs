using Dapper;
using PokerLibrary;
using PokerLibrary.Models;
using PokerTimer6.Data.Interfaces;
using PokerTimer6.Models;
using System;
using System.Data.SqlClient;
using System.Runtime.Intrinsics.Arm;

namespace PokerTimer6.Data;

/// <summary>
/// Shared tournament state – intentionally registered as Singleton.
/// 
/// This service holds the single source of truth for the entire poker tournament.
/// All connected clients (director screen, phones, tablets, projector) must see 
/// exactly the same data in real time. Using Singleton is not only acceptable here —
/// it is the correct and intended lifetime for a multi-user tournament director tool.
/// 
/// Do not change to Scoped or Transient — that would break real-time synchronization.
/// </summary>
public class GameService(IPlayerService playerService, IPayoutService payoutService, IDataAccess data) : IGameService
{
    public Queue<Round> Rounds { get; set; } = new Queue<Round>();
    public Round CurrentRound { get; set; } = new ();
    public int TournamentID { get; set; } = 1;
    public List<int> TournamentList { get; set; } = new List<int>();
    public event Func<Task>? OnChange;
    private readonly object _roundsLock = new();
    private int _roundCounter = 0;
    private readonly IPlayerService playerService = playerService;
    private readonly IPayoutService payoutService = payoutService;
    private readonly IDataAccess data = data;
    protected async void NotifyDataChanged()
    {
        if (OnChange is not null)
        {
            foreach (var handler in OnChange.GetInvocationList().Cast<Func<Task>>())
            {
                try
                {
                    await handler();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"OnChange handler thre: {ex}");
                }
            }
        }
    }
            
    /// <summary>
    /// Gets next round from the queue
    /// </summary>
    public void SetCurrentRound()
    {
        lock(_roundsLock)
        {
            if (Rounds.Count > 0) CurrentRound = Rounds.Dequeue();
        }
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
        if (roundModel == null) throw new ArgumentNullException(nameof(roundModel));

        var bigBlind = roundModel.BigBlind ?? 0;
        if (bigBlind < 0) throw new ArgumentOutOfRangeException(nameof(roundModel.BigBlind), "BigBlind must be non-negative");

        var roundMinutes = roundModel.RoundMinutes;
        if (roundMinutes < 0) throw new ArgumentOutOfRangeException(nameof(roundModel.RoundMinutes), "RoundMinutes must be non-negative");

        var smallBlind = (int)(bigBlind / 2);
        var roundTime = TimeSpan.FromMinutes((int)roundMinutes!);  // already tested for 0

        var newRoundNumber = Interlocked.Increment(ref _roundCounter);
        var newRound = new Round
        {
            id = roundModel.id,
            Tournament_id = roundModel.Tournament_id,
            RoundNumber = newRoundNumber,
            SmallBlind = smallBlind,
            BigBlind = bigBlind,
            RoundTime = roundTime,
            Time = (int)roundMinutes,
            RoundMinutes = roundMinutes

        };
        lock (_roundsLock)
        {
            Rounds.Enqueue(newRound); ;
        }
        NotifyDataChanged();
    }
    /// <summary>
    /// Deletes Round from the queue
    /// </summary>
    /// <param name="round">Round to delete</param>
    public void RemoveRound(Round round)
    {
        if (round is null) return;
        lock(_roundsLock)
        { Rounds = new Queue<Round>(Rounds.Where(r => r != round)); }
        NotifyDataChanged();
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
        var idParam = new DynamicParameters();
        idParam.Add("@TournamentID", TournamentID);
        var IdList = await data.LoadData<Round>($"select id from Rounds where Tournament_id = @TournamentID", idParam );
        List<Round> updateList;
        List<Round> insertList;
        List<int> deleteIds;
        
        lock(_roundsLock)
        {
            updateList = (from r in Rounds where IdList.Any(i => i.id == r.id) select r).ToList();// Gets the rounds that need to be updated
            insertList = Rounds.Where(r => !(IdList.Select(i=> i.id).Contains(r.id))).ToList();  // Gets the rounds to be inserted
            deleteIds = IdList.Where(i => !Rounds.Select(r => r.id).Contains(i.id)).Select(i=>i.id).ToList();      // Gets the list of id's to be deleted
        }
        await data.ExecuteInTransactionAsync(async (connection, transaction) =>
        {
            var updateSql = $"update Rounds set BigBlind = @BigBlind, Time = @Time, Tournament_id = @TournamentID where id = @id";
            foreach (var item in updateList)
            {
                var p = new DynamicParameters();
                p.Add("@id", item.id);
                p.Add("@BigBlind", item.BigBlind);
                p.Add("@Time", (int)item.RoundTime.TotalMinutes);
                p.Add("TournamentID", TournamentID);
                await connection.ExecuteAsync(updateSql, p, transaction);
            }

            var insertSql = $"insert into Rounds (BigBlind, Time, Tournament_id) values (@BigBlind, @Time, @TournamentID)";
            foreach (var item in insertList)
            {
                var p = new DynamicParameters();
                p.Add("@BigBlind", item.BigBlind);
                p.Add("@Time", (int)item.RoundTime.TotalMinutes);
                p.Add("TournamentID", TournamentID);
                await connection.ExecuteAsync(insertSql, p, transaction);
            }
            if (deleteIds.Any())
            {
                var deleteSql = "delete from Rounds where id IN @Ids";
                await connection.ExecuteAsync(deleteSql, new { Ids = deleteIds }, transaction);
            }
        });
    }
    /// <summary>
    /// Creates a new tournament_id in the Round table with the current blind structure.
    /// </summary>
    /// <returns></returns>
    public async Task NewRoundLayoutAsync()
    {
        var newTournamentId = (TournamentList.Any()) ? TournamentList.Max() + 1 : 1;
        TournamentList.Add(newTournamentId);
        
        List<object> insertParams;
        lock(_roundsLock)
        {
            insertParams = Rounds.Select(r => new
            {
                BigBlind = r.BigBlind,
                Time = (int)r.RoundTime.TotalMinutes,
                TournamentID = newTournamentId
            }).Cast<object>().ToList();
        }
        if (!insertParams.Any()) return;

        await data.ExecuteInTransactionAsync(async (conn, tx) =>
        {
            var insertSql = "insert into Rounds (BigBlind, Time, Tournament_id) values (@BigBlind, @Time, @TournamentID)";
            foreach (var item in insertParams)
            {
                await conn.ExecuteAsync(insertSql, item, tx);
            }
        });            
    }
    /// <summary>
    /// Loads the blind structure of the selected tournament_id (from UI)
    /// </summary>
    /// <returns></returns>
    public async Task LoadRoundLayoutAsync()
    {
        lock(_roundsLock)
        {
            Rounds.Clear();
            _roundCounter = 0;
        }
        var param = new DynamicParameters();
        param.Add("@TournamentID", TournamentID);
        var output = await data.LoadData<Round>($"select id, BigBlind, Time, Tournament_id from Rounds where Tournament_id = @TournamentID", param);
        foreach(var item in output)
        {
            item.RoundMinutes = item.Time;
            var clone = new Round
            {
                id = item.id,
                Tournament_id = item.Tournament_id,
                SmallBlind = (int)(item.BigBlind ?? 0 / 2),
                BigBlind = item.BigBlind,
                RoundTime = item.RoundTime,
                Time = item.Time,
                RoundMinutes = item.RoundMinutes
            };
            lock (_roundsLock)
            {
                AddRound(clone);
            }
        }
        NotifyDataChanged();
    }
    /// <summary>
    /// Gets the list of tournament #s from the database (Round table) with blind structures saved.
    /// </summary>
    /// <returns></returns>
    public async Task LoadTournamentListAsync()
    {        
        var output = await data.LoadData<int>("select distinct Tournament_id from Rounds", new DynamicParameters());
        TournamentList = output.ToList();
    }
}
