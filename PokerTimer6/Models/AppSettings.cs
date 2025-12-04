namespace PokerTimer6.Models
{
    public record AppSettings
    {
        public required IReadOnlyList<PayoutBracket> Payouts { get; init; }
        public required int RoundPayoutsToNearest { get; init; }

        public decimal BuyIn { get; init; } = 100m;
        public decimal RebuyAmount { get; init; } = 100m;
        public decimal AddOnAmount { get; init; } = 100m;
        public int StartingStack { get; init; } = 15000;
        public int RebuyStack { get; init; } = 15000;
        public int AddOnStack { get; init; } = 15000;
    }
}
