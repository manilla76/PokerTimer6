namespace PokerTimer6.Models
{
    public class Round
    {
        public uint SmallBlind { get; set; }
        public uint BigBlind { get; set; }
        public uint RoundNumber { get; set; }
        public TimeSpan RoundTime { get; set; }  // probably should be in TimeOffset
        public int Round_id { get; set; }
        public int Tournament_id { get; set; }
        public int Time { get; set; }


    }
}
