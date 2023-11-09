using PokerTimer6.Models;

namespace PokerTimer6.Data.Interfaces
{
    public interface IPayoutService
    {
        Payout ActivePayout { get; }
        List<Payout> Payouts { get; set; }
        uint PrizeMoney { get; }
        int RoundPayoutsToNearest { get; set; }

        event Action? OnChange;

        void AddPrizeMoney();
        void CalculatePayout();
        void ResetPayout();
        void SetActivePayout(uint StartingNumberOfPlayers);
    }
}