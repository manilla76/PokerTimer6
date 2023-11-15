using PokerLibrary.Data.Interfaces;
using PokerLibrary.Models;

namespace PokerLibrary.Data
{
    public class PlayerService : IPlayerService
    {
        private Random rng = new Random();
        public List<int> Dealers { get; set; } = new List<int>();
        public uint NextPlayerId { get; private set; }
        public List<Player> Players { get; set; } = new List<Player>();
        public List<Player> ActiveList { get; set; } = new List<Player>();
        public uint StartingNumberOfPlayers { get; private set; }

        public event Action? OnChange;

        private void NotifyDataChanged() => OnChange?.Invoke();

        /// <summary>
        /// Get next PlayerId to ensure each player gets a unique id#.  This probably should come from the database eventually
        /// </summary>
        /// <returns>ID</returns>
        public uint GetNextPlayerId()
        {
            NextPlayerId++;
            return NextPlayerId;
        }
        public async Task SetNextPlayerID(IGameService game)
        {
            NextPlayerId = await game.GetNextID();
        }
        /// <summary>
        /// Add to player list
        /// </summary>
        /// <param name="player">Player</param>
        public void AddPlayer(Player player)
        {
            Players.Add(player);  // probably should check for duplicates
            NotifyDataChanged();  // when something important changes notify UI of updates

            //string sql = @"Insert into Players (Name, Table, Seat) Values(@Name, @Table, @Seat)";
            //DataAccess.Add(sql, DataAccess.GetConstructionString());
        }
        /// <summary>
        /// Remove Player from Players
        /// </summary>
        /// <param name="player">Player</param>
        public void RemovePlayer(Player player)
        {
            Players.Remove(player);
            NotifyDataChanged();
        }
        /// <summary>
        /// Shuffle players and assign to random table/seat.  Assign first dealer for each table
        /// </summary>
        public void ShufflePlayers()
        {
            // determine the number of players starting the tournament
            if (ActiveList.Count == 0)
            {
                StartingNumberOfPlayers = (uint)Players.Count(p => p.IsActive);
            }
            // get all players still playing
            ActiveList = Players.Where(p => p.IsActive).ToList();
            if (ActiveList.Count == 0) return;
            // shuffle all active players
            Shuffle<Player>(ActiveList);
            // assign seats and return number of active tables
            uint tableCount = AsignSeats();
            // reset dealers for each table
            Dealers.Clear();
            for (int i = 1; i <= tableCount; i++)
            {
                SetDealer(i);
            }
            //StateHasChanged();
            NotifyDataChanged();

        }
        /// <summary>
        /// Asign players to table/seat combo
        /// </summary>
        /// <returns>number of active tables</returns>
        private uint AsignSeats()
        {
            // get number of tables needed
            uint tableCount = (uint)(ActiveList.Count - 1) / 10 + 1;
            // assign each player to a table/seat combo.  Balance # of players at each table
            if (tableCount > 0 & ActiveList.Count > 0)
            {
                for (int i = 0; i < ActiveList.Count; i++)
                {
                    ActiveList[i].Player_Seat.TableNumber = (uint)i % tableCount + 1;
                    ActiveList[i].Player_Seat.SeatNumber = (uint)i / tableCount + 1;
                }
            }

            return tableCount;
        }
        /// <summary>
        /// For each table, choose a random first dealer
        /// </summary>
        /// <param name="table">Table #</param>
        public void SetDealer(int table)
        {
            if (Dealers.Count >= table)
            {
                Dealers[table - 1] = rng.Next(1, ActiveList.Count(p => p.Player_Seat.TableNumber == table) + 1); // update table dealer
            }
            else
            {
                Dealers.Add(rng.Next(1, ActiveList.Count(p => p.Player_Seat.TableNumber == table) + 1));  // Set dealer per table 
            }
            NotifyDataChanged();
        }
        /// <summary>
        /// For players added after game is started, add player to the game.  Balance the number of players on each table.
        /// If a new table is needed, add a table and reshuffle players.  
        /// </summary>
        public void SetSeat()
        {
            // get player without a seat
            var playerToSeat = Players.Find(p => p.Player_Seat.SeatNumber == 0);
            if (playerToSeat == null)
                return;
            //add to active list
            ActiveList.Add(playerToSeat);

            if (ActiveList.Count % 10 == 1)  // If a new table is needed, shuffle all players
            {
                ShufflePlayers();
            }
            else
            {
                // find the next open seat keeping all tables balanced
                // get # of tables
                // If this person fills the last table, fill it.  Otherwise, add to another table.

                int numberOfTables = (int)Math.Ceiling((double)ActiveList.Count / 10);  // get number of tables
                if (ActiveList.Count % numberOfTables == 0)         // new player fills the last table
                {
                    playerToSeat.Player_Seat.TableNumber = (uint)numberOfTables;

                }
                else   //  find the first opening and fill it.
                {
                    playerToSeat.Player_Seat.TableNumber = (uint)(ActiveList.Count % numberOfTables);
                    // playerToSeat.Player_Seat.SeatNumber = (uint)ActiveList.Count / (uint)numberOfTables + 1;
                }
                playerToSeat.Player_Seat.SeatNumber = (uint)Math.Ceiling((double)ActiveList.Count / (double)numberOfTables);
            }

            StartingNumberOfPlayers++;  // add to starting number of players (to be used to determine payout structure)
        }
        /// <summary>
        /// Randomize the order of the list of objects
        /// </summary>
        /// <typeparam name="T">Data Model of list to shuffle</typeparam>
        /// <param name="list">List to randomized</param>
        public void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        /// <summary>
        /// Reset players, startingNumberOfPlayers.
        /// </summary>
        public void ResetPlayers()
        {
            ActiveList.Clear();
            StartingNumberOfPlayers = 0;
            NotifyDataChanged();
        }

    }
}
