namespace PokerTimer6.Data.Interfaces
{
    public interface ITimerService
    {
        string DisplayValue { get; set; }
        string IconString { get; }
        bool IsRunning { get; set; }
        bool SoundAlert { get; }
        bool TimerEmpty { get; }

        event Func<Task>? OnChange;

        void ChangeIcon();
        void Dispose();
        Task SetTimeRemaining(TimeSpan time);
        Task SetTimeRemaining();
        Task StartPauseAsync();
        Task StopAsync();
    }
}