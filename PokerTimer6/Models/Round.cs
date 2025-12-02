using System.ComponentModel.DataAnnotations;

namespace PokerTimer6.Models
{
    public class Round
    {
        public int SmallBlind { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Value must be > 0.")]
        public int? BigBlind { get; set; }
        public int RoundNumber { get; set; }
        public TimeSpan RoundTime { get; set; }  // probably should be in TimeOffset

        [Range(1, 1000, ErrorMessage = "Value must be in minutes between 1 and 1000.")]
        public int? RoundMinutes { get; set; }
        public int id { get; set; }
        public int Tournament_id { get; set; }
        public int Time { get; set; }


    }
}
