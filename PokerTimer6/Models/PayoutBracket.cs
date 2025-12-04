namespace PokerTimer6.Models
{

    public record PayoutBracket
    {
        public required int MinNumberOfPlayers { get; init; }
        public required int MaxNumberOfPlayers { get; init; } 
        public required IReadOnlyList<decimal> PayoutPercents { get; init; }
        public List<int> Payouts { get; init; } = new List<int>();
    }
    
}
