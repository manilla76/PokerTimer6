using PokerLibrary;
using PokerTimer6.Models;

namespace PokerTimer6.Data
{
    public class GameService
    {
        public List<Player> Players { get; set; } = new List<Player>();
        public List<Player> ActiveList { get; set; } = new List<Player>();
        public uint PrizeMoney { get; set; }
        public List<Payout> Payouts { get; set; } = new ();
        public Payout? ActivePayout { get; private set; } = new();
        public Queue<Round> Rounds { get; set; } = new Queue<Round>();

        public Round? CurrentRound;
        public uint StartingNumberOfPlayers { get; set; }
        public int RoundPayoutsToNearest { get; set; }


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
    }

}
