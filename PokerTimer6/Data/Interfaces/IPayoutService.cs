using PokerTimer6.Models;

namespace PokerTimer6.Data.Interfaces
{
    public interface IPayoutService
    {
        PayoutBracket ActivePayout { get; }
        uint PrizeMoney { get; }
        uint BuyIn { get; set; }
        event Func<Task>? OnChange;

        void AddPrizeMoney();
        void CalculatePayout();
        void ResetPayout();
        void SetActivePayout(uint StartingNumberOfPlayers);
    }
}