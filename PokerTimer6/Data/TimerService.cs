using Microsoft.AspNetCore.Components;
using PokerTimer6.Data.Interfaces;
using PokerTimer6.Models;

namespace PokerTimer6.Data
{
    public class TimerService : ITimerService, IDisposable
    {
        public event Action? OnChange;
        private void NotifyDataChanged() => OnChange?.Invoke();

        private static event EventHandler<TimerEventArgs>? OnTimerChanged;
        private static TimeSpan timeRemaining { get; set; }
        public bool TimerEmpty { get => timeRemaining.Seconds <= 0; }
        public string IconString { get; private set; } = string.Empty;

        private bool isRunning = false;
        public bool IsRunning
        {
            get => isRunning;
            set { isRunning = value; NotifyDataChanged(); }
        }
        private string displayValue = "oi oi-media-play";
        public string DisplayValue
        {
            get => displayValue;
            set { displayValue = value; NotifyDataChanged(); }
        }
        public bool SoundAlert { get => timeRemaining.TotalSeconds <= 0 & !isRunning; }
        /// <summary>
        /// 1 second timer to be used by the timer display to identify the clock time should change
        /// </summary>
        private static Timer InternalTimer = new Timer((state) =>
        {
            timeRemaining = timeRemaining.Subtract(new TimeSpan(0, 0, 0, 1));
            if (OnTimerChanged is not null)
            {
                OnTimerChanged.Invoke(null, new TimerEventArgs(timeRemaining));
            }
        });
        /// <summary>
        /// Play/Pause.  If currently playing, pause.  If currently paused, play
        /// </summary>
        /// <returns></returns>
        public Task StartPauseAsync()
        {
            IsRunning = !IsRunning;
            if (IsRunning & timeRemaining.TotalSeconds > 0)
                InternalTimer.Change(1000, 1000);
            else
                StopAsync();

            return Task.CompletedTask;
        }
        /// <summary>
        /// Stop the timer
        /// </summary>
        /// <returns></returns>
        public Task StopAsync()
        {
            IsRunning = false;
            InternalTimer.Change(Timeout.Infinite, Timeout.Infinite);
            NotifyDataChanged();
            return Task.CompletedTask;
        }

        public TimerService()
        {
            //subscribe to timer event
            OnTimerChanged += async (o, e) =>
            {
                // Since we're not necessarily on the thread that has proper access to the renderer context
                // we need to use the InvokeAsync() method, which takes care of running our code on the right thread.
                await CalculateDisplayValue();
            };

            // Calculate the initial value for the timer.
            CalculateDisplayValue();

        }
        /// <summary>
        /// Set time remaining to 10 seconds?  Not sure if this is actually used.  Probably can go away.
        /// </summary>
        /// <returns></returns>
        public async Task SetTimeRemaining()
        {
            timeRemaining = new TimeSpan(0, 0, 0, 10);
            await CalculateDisplayValue();
        }
        /// <summary>
        /// Set time remaining to the time input
        /// </summary>
        /// <param name="time">time to set the timer to</param>
        /// <returns></returns>
        public async Task SetTimeRemaining(TimeSpan time)
        {
            timeRemaining = time;
            await CalculateDisplayValue();
        }
        /// <summary>
        /// Set the display string based on the current timer
        /// </summary>
        /// <returns></returns>
        private async Task CalculateDisplayValue()
        {
            DisplayValue = timeRemaining.Hours > 0 ? timeRemaining.ToString("h\\:mm\\:ss") : timeRemaining.ToString("mm\\:ss");
            if (timeRemaining.TotalSeconds <= 0)
                await StopAsync();
        }
        /// <summary>
        /// Set the string for play/pause icon based on the current status of the timer
        /// </summary>
        public void ChangeIcon()
        {
            IconString = (IsRunning) ? "oi oi-media-pause" : "oi oi-media-play";
            //PlayButtonDisabled = TimerService.TimerEmpty;
        }
        /// <summary>
        /// Unsub from the timer event
        /// </summary>
        public void Dispose()
        {
            //unsub to timer event
            OnTimerChanged -= async (o, e) =>
            {
                // Since we're not necessarily on the thread that has proper access to the renderer context
                // we need to use the InvokeAsync() method, which takes care of running our code on the right thread.
                await CalculateDisplayValue();
            };
        }
    }
}
