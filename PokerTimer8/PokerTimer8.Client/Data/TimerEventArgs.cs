
namespace PokerTimer8.Client.Data
{
    internal class TimerEventArgs
    {
        private TimeSpan timeRemaining;

        public TimerEventArgs(TimeSpan timeRemaining)
        {
            this.timeRemaining = timeRemaining;
        }
    }
}