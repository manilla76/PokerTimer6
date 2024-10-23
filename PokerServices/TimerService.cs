using PokerServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServices
{
    public class TimerService : ITimerService, IDisposable
    {
        public Timer TTimer { get; init; }

        public TimerService()
        {
            TTimer = new Timer(TickTimer, null, 1000, 1000);
        }
        private void TickTimer(object? state) => TimeRemaining = TimeRemaining.Subtract(tick);
        private TimeSpan tick = TimeSpan.FromSeconds(1);
        public TimeSpan OriginalTimeSpan { get; private set; } = TimeSpan.Zero;
        public TimeSpan TimeRemaining { get; private set; } = TimeSpan.Zero;

        public bool IsRunning { get; private set; }

        public void SetTime(TimeSpan time) => OriginalTimeSpan = TimeRemaining = time;
        public void SetTime(double minutes) => SetTime(TimeSpan.FromMinutes(minutes));

        public void ResetTimer() => TimeRemaining = OriginalTimeSpan;
        
        public void StartTimer()
        {
            TTimer.Change(1000, 1000);
            IsRunning = true;
        }

        public void StopTimer()
        {
            TTimer.Change(Timeout.Infinite, Timeout.Infinite);
            IsRunning = false;
        }

        public void Dispose() => TTimer.Dispose();
        
    }
}
