using PokerTimer6.Models;

namespace PokerTimer6.Data
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

        public void AddPrizeMoney()
        {
            PrizeMoney += BuyIn;
            CalculatePayout();
            NotifyDataChanged();
        }
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
        public void CalculatePayout()
        {
            ActivePayout.Payouts.Clear();
            if (ActivePayout.PayoutPercents is null)
            {
                return;
            }
            for (int i = 0; i < ActivePayout.PayoutPercents.Count - 1; i++)
            {
                var payout = ActivePayout.PayoutPercents[i] * PrizeMoney;
                int roundedpayout = (int)(RoundPayoutsToNearest * Math.Round(payout / (float)RoundPayoutsToNearest));
                ActivePayout.Payouts.Add(roundedpayout);
            }

            ActivePayout.Payouts.Add((int)PrizeMoney - ActivePayout.Payouts.Sum());

        }

        public void ResetPayout()
        {
            PrizeMoney = 0;
            ActivePayout = new Payout();
            NotifyDataChanged();
        }
    }
}
