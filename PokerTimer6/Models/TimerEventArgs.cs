namespace PokerTimer6.Models
{
    public class TimerEventArgs : EventArgs
    {
        public TimerEventArgs(TimeSpan time)
        {
            Time = time;
        }
        public TimeSpan Time { get; set; }
    }
}
