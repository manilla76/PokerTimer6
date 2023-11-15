namespace PokerTimer6.Data
{
    public sealed record Continent
    {
        public Guid Uid { get; init; } = Guid.NewGuid();
        public required string Name { get; init; }
    }
}
