namespace PokerTimer6.Models
{
    public class Payout
    {
        public uint MinNumberOfPlayers { get; set; }
        public uint MaxNumberOfPlayers { get; set; }
        public List<float> PayoutPercents { get; set; }

        public List<int> Payouts { get; set; } = new List<int>();
        
    }
}
