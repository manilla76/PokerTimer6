using PokerTimer6.Models;
using PokerTimer6.Pages;

namespace PokerTimer6.Data
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

        public uint GetNextPlayerId()
        {
            NextPlayerId++;
            return NextPlayerId;
        }
        public void AddPlayer(Player player)
        {
            Players.Add(player);  // probably should check for duplicates
            NotifyDataChanged();  // when something important changes notify UI of updates

            //string sql = @"Insert into Players (Name, Table, Seat) Values(@Name, @Table, @Seat)";
            //DataAccess.Add(sql, DataAccess.GetConstructionString());
        }

        public void RemovePlayer(Player player)
        {
            Players.Remove(player);
            NotifyDataChanged();
        }

        public void ShufflePlayers()
        {
            if (ActiveList.Count == 0)
            {
                StartingNumberOfPlayers = (uint)Players.Count(p => p.IsActive);
            }
            ActiveList = Players.Where(p => p.IsActive).ToList();
            if (ActiveList.Count == 0) return;
            Shuffle<Player>(ActiveList);
            uint tableCount = (uint)(ActiveList.Count - 1) / 10 + 1;
            if (tableCount > 0 & ActiveList.Count > 0)
            {
                for (int i = 0; i < ActiveList.Count; i++)
                {
                    ActiveList[i].Player_Seat.TableNumber = (uint)i % tableCount + 1;
                    ActiveList[i].Player_Seat.SeatNumber = (uint)i / tableCount + 1;
                }
            }
            Dealers.Clear();
            for (int i = 1; i <= tableCount; i++)
            {
                SetDealer(i);
            }
            //StateHasChanged();
            NotifyDataChanged();

        }
        public void SetDealer(int table)
        {
            if (Dealers.Count >= table)
            {
                Dealers[table - 1] = rng.Next(1, ActiveList.Count(p => p.Player_Seat.TableNumber == table) + 1);
            }
            else
            {
                Dealers.Add(rng.Next(1, ActiveList.Count(p => p.Player_Seat.TableNumber == table) + 1));  // Set dealer per table 
            }
            NotifyDataChanged();
        }

        public void SetSeat()
        {
            var playerToSeat = Players.Find(p => p.Player_Seat.SeatNumber == 0);
            if (playerToSeat == null)
                return;
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

                int numberOfTables = (int)Math.Ceiling((double)ActiveList.Count / 10);
                if (ActiveList.Count % numberOfTables == 0)
                {
                    playerToSeat.Player_Seat.TableNumber = (uint)numberOfTables;

                }
                else
                {
                    playerToSeat.Player_Seat.TableNumber = (uint)(ActiveList.Count % numberOfTables);
                    // playerToSeat.Player_Seat.SeatNumber = (uint)ActiveList.Count / (uint)numberOfTables + 1;
                }
                playerToSeat.Player_Seat.SeatNumber = (uint)Math.Ceiling((double)ActiveList.Count / (double)numberOfTables);
            }

            StartingNumberOfPlayers++;
        }
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
        public void ResetPlayers()
        {
            ActiveList.Clear();
            StartingNumberOfPlayers = 0;
            NotifyDataChanged();
        }
    }
}
