using Microsoft.Extensions.Options;
using PokerTimer6.Data.Interfaces;
using PokerTimer6.Models;

namespace PokerTimer6.Data
{
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
    public class PayoutService : IPayoutService
    {
        public uint PrizeMoney { get; private set; }
        public static uint BuyIn { get; set; } = 50;
        
        private readonly AppSettings settings;
        private readonly IReadOnlyList<PayoutBracket> brackets;

        public PayoutService(IOptions<AppSettings> options)
        {
            settings = options.Value;
            brackets = settings.Payouts;
        }
        public PayoutBracket ActivePayout { get; private set; }

        public event Func<Task>? OnChange;
        protected async void NotifyDataChanged()
        {
            if (OnChange is not null) await Task.WhenAll
            (OnChange.GetInvocationList().Cast<Func<Task>>().Select(x => x()));
        }

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
            ActivePayout = brackets.First(p => p.MinNumberOfPlayers <= StartingNumberOfPlayers & p.MaxNumberOfPlayers >= StartingNumberOfPlayers);
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
                int roundedpayout = (int)(settings.RoundPayoutsToNearest * Math.Round(payout / (decimal)settings.RoundPayoutsToNearest));
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
            ActivePayout.Payouts.Clear();
            NotifyDataChanged();
        }
    }
}
