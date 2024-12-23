using PokerTimer6.Data.Interfaces;
using PokerTimer6.Models;
using PokerTimer6.Pages;

namespace PokerTimer6.Data
{
    /// <summary>
    /// Service for managing player operations such as adding, removing, shuffling, and seating players.
    /// </summary>
    public class PlayerService : IPlayerService
    {
        private readonly Random rng = new();
        public List<int> Dealers { get; set; } = new();
        public uint NextPlayerId { get; private set; }
        public List<Player> Players { get; set; } = new();
        public List<Player> ActiveList { get; set; } = new();
        public uint StartingNumberOfPlayers { get; private set; }

        public event Action? OnChange;

        private void NotifyDataChanged() => OnChange?.Invoke();

        /// <summary>
        /// Gets the next player ID to ensure each player gets a unique ID.
        /// </summary>
        /// <returns>The next player ID.</returns>
        public uint GetNextPlayerId()
        {
            return ++NextPlayerId;
        }

        /// <summary>
        /// Sets the next player ID from the game service.
        /// </summary>
        /// <param name="game">The game service.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SetNextPlayerID(IGameService game)
        {
            NextPlayerId = await game.GetNextID();
        }

        /// <summary>
        /// Adds a player to the player list.
        /// </summary>
        /// <param name="player">The player to add.</param>
        public void AddPlayer(Player player)
        {
            if (!Players.Contains(player))
            {
                Players.Add(player);
                NotifyDataChanged();
            }
        }

        /// <summary>
        /// Removes a player from the player list.
        /// </summary>
        /// <param name="player">The player to remove.</param>
        public void RemovePlayer(Player player)
        {
            if (Players.Remove(player))
            {
                NotifyDataChanged();
            }
        }

        /// <summary>
        /// Shuffles the players and assigns them to random tables and seats.
        /// </summary>
        public void ShufflePlayers()
        {
            if (ActiveList.Count == 0)
            {
                StartingNumberOfPlayers = (uint)Players.Count(p => p.IsActive);
            }

            ActiveList = Players.Where(p => p.IsActive).ToList();
            if (ActiveList.Count == 0) return;

            Shuffle(ActiveList);
            uint tableCount = AssignSeats();
            Dealers.Clear();
            for (int i = 1; i <= tableCount; i++)
            {
                SetDealer(i);
            }
            NotifyDataChanged();
        }

        /// <summary>
        /// Assigns players to tables and seats.
        /// </summary>
        /// <returns>The number of active tables.</returns>
        private uint AssignSeats()
        {
            uint tableCount = (uint)(ActiveList.Count - 1) / 10 + 1;
            for (int i = 0; i < ActiveList.Count; i++)
            {
                ActiveList[i].Player_Seat.TableNumber = (uint)i % tableCount + 1;
                ActiveList[i].Player_Seat.SeatNumber = (uint)i / tableCount + 1;
            }
            return tableCount;
        }

        /// <summary>
        /// Sets a random dealer for each table.
        /// </summary>
        /// <param name="table">The table number.</param>
        public void SetDealer(int table)
        {
            int dealerIndex = rng.Next(1, ActiveList.Count(p => p.Player_Seat.TableNumber == table) + 1);
            if (Dealers.Count >= table)
            {
                Dealers[table - 1] = dealerIndex;
            }
            else
            {
                Dealers.Add(dealerIndex);
            }
            NotifyDataChanged();
        }

        /// <summary>
        /// Sets a seat for a player added after the game has started.
        /// </summary>
        public void SetSeat()
        {
            var playerToSeat = Players.Find(p => p.Player_Seat.SeatNumber == 0);
            if (playerToSeat == null) return;

            ActiveList.Add(playerToSeat);

            if (ActiveList.Count % 10 == 1)
            {
                ShufflePlayers();
            }
            else
            {
                int numberOfTables = (int)Math.Ceiling((double)ActiveList.Count / 10);
                playerToSeat.Player_Seat.TableNumber = (uint)(ActiveList.Count % numberOfTables == 0 ? numberOfTables : ActiveList.Count % numberOfTables);
                playerToSeat.Player_Seat.SeatNumber = (uint)Math.Ceiling((double)ActiveList.Count / numberOfTables);
            }

            StartingNumberOfPlayers++;
        }

        /// <summary>
        /// Shuffles a list of items.
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="list">The list to shuffle.</param>
        public void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        /// <summary>
        /// Resets the players and the starting number of players.
        /// </summary>
        public void ResetPlayers()
        {
            ActiveList.Clear();
            StartingNumberOfPlayers = 0;
            NotifyDataChanged();
        }
    }
}
