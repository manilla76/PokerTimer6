using PokerLibrary.Data.Interfaces;
using PokerLibrary.Models;

namespace PokerLibrary.Data
{
    public class PayoutService : IPayoutService
    {
        public uint PrizeMoney { get; private set; }
        public static uint BuyIn { get; set; } = 50;
        public List<Payout> Payouts { get; set; } = new();
        public Payout ActivePayout { get; private set; } = new();
        public int RoundPayoutsToNearest { get; set; }

        public event Action? OnChange;

        private void NotifyDataChanged() => OnChange?.Invoke();

        /// <summary>
        /// Add current BuyIn (from UI) to the prize pool
        /// </summary>
        public void AddPrizeMoney()
        {
            PrizeMoney += BuyIn;
            CalculatePayout();
            NotifyDataChanged();
        }
        /// <summary>
        /// Look up the payout structure from the configuration (appsettings.json)
        /// </summary>
        /// <param name="StartingNumberOfPlayers"># of players</param>
        public void SetActivePayout(uint StartingNumberOfPlayers)
        {
            ActivePayout = Payouts.First(p => p.MinNumberOfPlayers <= StartingNumberOfPlayers & p.MaxNumberOfPlayers >= StartingNumberOfPlayers);
            if (PrizeMoney == 0)
            {
                PrizeMoney = StartingNumberOfPlayers * BuyIn;   // If this is the first time, initialize prizemoney.
            }
            CalculatePayout();
            NotifyDataChanged();
        }
        /// <summary>
        /// Calculate the payout based on the prize pool and payout structure
        /// </summary>
        public void CalculatePayout()
        {
            ActivePayout.Payouts.Clear();
            if (ActivePayout.PayoutPercents is null)
            {
                return;
            }
            // set payout for each level close to the payout percentage and rounded to the nearest 10.
            for (int i = 0; i < ActivePayout.PayoutPercents.Count - 1; i++)
            {
                var payout = ActivePayout.PayoutPercents[i] * PrizeMoney;
                int roundedpayout = (int)(RoundPayoutsToNearest * Math.Round(payout / (float)RoundPayoutsToNearest));
                ActivePayout.Payouts.Add(roundedpayout);
            }

            ActivePayout.Payouts.Add((int)PrizeMoney - ActivePayout.Payouts.Sum());

        }

        /// <summary>
        /// Reset Payout
        /// </summary>
        public void ResetPayout()
        {
            PrizeMoney = 0;
            ActivePayout = new Payout();
            NotifyDataChanged();
        }
    }
}
