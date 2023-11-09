namespace PokerTimer6.Data.Interfaces
{
    public interface ITimerService
    {
        string DisplayValue { get; set; }
        string IconString { get; }
        bool IsRunning { get; set; }
        bool SoundAlert { get; }
        bool TimerEmpty { get; }

        event Action OnChange;

        void ChangeIcon();
        void SetTimeRemaining();
        void SetTimeRemaining(TimeSpan time);
        Task StartPauseAsync();
        Task StopAsync();
    }
}