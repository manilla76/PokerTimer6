using PokerLibrary;
using PokerTimer6.Models;

namespace PokerTimer6.Data
{
    public class GameService
    {
        public List<Player> Players { get; set; } = new List<Player>();
        public List<Player> ActiveList { get; set; } = new List<Player>();
        public List<int> Dealers { get; set; } = new List<int>();
        public uint PrizeMoney { get; set; }
        public List<Payout> Payouts { get; set; } = new ();
        public Payout? ActivePayout { get; private set; } = new();
        public Queue<Round> Rounds { get; set; } = new Queue<Round>();

        public Round? CurrentRound;
        public uint StartingNumberOfPlayers { get; set; }
        public int RoundPayoutsToNearest { get; set; }
        private Random rng = new Random();
        //private List<int> dealers = new();

        private uint NextPlayerId = 0;           
        public static uint BuyIn { get; set; } = 50;

        public event Action? OnChange;

        private void NotifyDataChanged() => OnChange?.Invoke();
        public GameService()
        {
        }

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

        public void AddPrizeMoney()
        {
            PrizeMoney += BuyIn;
            CalculatePayout();
            NotifyDataChanged();
        }
        public void SetActivePayout()
        {
            ActivePayout = Payouts.First(p => p.MinNumberOfPlayers <= StartingNumberOfPlayers & p.MaxNumberOfPlayers >= StartingNumberOfPlayers);
            PrizeMoney = StartingNumberOfPlayers * BuyIn;
            CalculatePayout();
            NotifyDataChanged();
        }
        public void CalculatePayout()
        {
            ActivePayout.Payouts.Clear();
            for (int i = 0; i < ActivePayout.PayoutPercents.Count - 1; i++)
            {
                var payout = ActivePayout.PayoutPercents[i] * PrizeMoney;
                int roundedpayout = (int) (RoundPayoutsToNearest * Math.Round(payout / (float)RoundPayoutsToNearest));
                ActivePayout.Payouts.Add(roundedpayout);
            }

            ActivePayout.Payouts.Add((int)PrizeMoney - ActivePayout.Payouts.Sum());

        }
        public void SetCurrentRound()
        {
            CurrentRound = Rounds.Dequeue();
            NotifyDataChanged();
        }

        public void ResetTournament()
        {
            ActiveList.Clear();
            StartingNumberOfPlayers = 0;
            PrizeMoney = 0;
            ActivePayout = new Payout();
            NotifyDataChanged();
        }

        public void ShufflePlayers()
        {
            if (ActiveList.Count == 0)
            {
                StartingNumberOfPlayers = (uint)Players.Count(p => p.IsActive);
                SetActivePayout();
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
            switch (ActiveList.Count)
            {
                case <= 10:
                    playerToSeat.Player_Seat.TableNumber = 1;
                    playerToSeat.Player_Seat.SeatNumber = (uint)ActiveList.Count;
                    break;
                case 11:
                    // Force a shuffle
                    ShufflePlayers();
                    break;
                case <= 20:
                    if (ActiveList.Count % 2 == 0) // add to table 2
                    {
                        playerToSeat.Player_Seat.TableNumber = 2;
                        playerToSeat.Player_Seat.SeatNumber = (uint)ActiveList.Count / 2;
                    }
                    else    // add to table 1
                    {
                        playerToSeat.Player_Seat.TableNumber = 1;
                        playerToSeat.Player_Seat.SeatNumber = (uint)ActiveList.Count / 2 + 1;
                    }
                    break;
                case 21:
                    //Force a shuffle
                    ShufflePlayers();
                    break;
                case <= 30:
                    if (ActiveList.Count % 3 == 0) // add to table 3
                    {
                        playerToSeat.Player_Seat.TableNumber = 3;
                        playerToSeat.Player_Seat.SeatNumber = (uint)ActiveList.Count / 3;
                    }
                    else if (ActiveList.Count % 3 == 2)  // add to table 2
                    {
                        playerToSeat.Player_Seat.TableNumber = 2;
                        playerToSeat.Player_Seat.SeatNumber = (uint)ActiveList.Count / 3 + 1;
                    }
                    else    // add to table 1
                    {
                        playerToSeat.Player_Seat.TableNumber = 1;
                        playerToSeat.Player_Seat.SeatNumber = (uint)ActiveList.Count / 3 + 1;
                    }
                    break;
                default:
                    break;
            }
            PrizeMoney += BuyIn;
            NotifyDataChanged();
        }
    }


}
