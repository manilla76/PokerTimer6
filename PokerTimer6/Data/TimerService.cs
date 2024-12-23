using Microsoft.AspNetCore.Components;
using PokerTimer6.Data.Interfaces;
using PokerTimer6.Models;

namespace PokerTimer6.Data
{
    /// <summary>
    /// Service for managing the timer functionality.
    /// </summary>
    public class TimerService : ITimerService, IDisposable
    {
        /// <summary>
        /// Event triggered when data changes.
        /// </summary>
        public event Action? OnChange;

        private void NotifyDataChanged() => OnChange?.Invoke();

        private static event EventHandler<TimerEventArgs>? OnTimerChanged;
        private static TimeSpan timeRemaining { get; set; }

        /// <summary>
        /// Gets a value indicating whether the timer is empty.
        /// </summary>
        public bool TimerEmpty => timeRemaining.Seconds <= 0;

        /// <summary>
        /// Gets the icon string for play/pause.
        /// </summary>
        public string IconString { get; private set; } = string.Empty;

        private bool isRunning = false;

        /// <summary>
        /// Gets or sets a value indicating whether the timer is running.
        /// </summary>
        public bool IsRunning
        {
            get => isRunning;
            set { isRunning = value; NotifyDataChanged(); }
        }

        private string displayValue = "oi oi-media-play";

        /// <summary>
        /// Gets or sets the display value of the timer.
        /// </summary>
        public string DisplayValue
        {
            get => displayValue;
            set { displayValue = value; NotifyDataChanged(); }
        }

        /// <summary>
        /// Gets a value indicating whether a sound alert should be played.
        /// </summary>
        public bool SoundAlert => timeRemaining.TotalSeconds <= 0 && !isRunning;

        private static readonly Timer InternalTimer = new Timer((state) =>
        {
            timeRemaining = timeRemaining.Subtract(TimeSpan.FromSeconds(1));
            OnTimerChanged?.Invoke(null, new TimerEventArgs(timeRemaining));
        });

        /// <summary>
        /// Toggles the timer between start and pause states.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task StartPauseAsync()
        {
            IsRunning = !IsRunning;
            if (IsRunning && timeRemaining.TotalSeconds > 0)
                InternalTimer.Change(1000, 1000);
            else
                await StopAsync();
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task StopAsync()
        {
            IsRunning = false;
            InternalTimer.Change(Timeout.Infinite, Timeout.Infinite);
            NotifyDataChanged();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerService"/> class.
        /// </summary>
        public TimerService()
        {
            OnTimerChanged += async (o, e) => await CalculateDisplayValue();
            CalculateDisplayValue();
        }

        /// <summary>
        /// Sets the time remaining to 10 seconds.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SetTimeRemaining()
        {
            timeRemaining = TimeSpan.FromSeconds(10);
            await CalculateDisplayValue();
        }

        /// <summary>
        /// Sets the time remaining to the specified time.
        /// </summary>
        /// <param name="time">The time to set the timer to.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SetTimeRemaining(TimeSpan time)
        {
            timeRemaining = time;
            await CalculateDisplayValue();
        }

        /// <summary>
        /// Calculates and sets the display value based on the current timer.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task CalculateDisplayValue()
        {
            DisplayValue = timeRemaining.Hours > 0 ? timeRemaining.ToString("h\\:mm\\:ss") : timeRemaining.ToString("mm\\:ss");
            if (timeRemaining.TotalSeconds <= 0)
                await StopAsync();
        }

        /// <summary>
        /// Sets the icon string based on the current status of the timer.
        /// </summary>
        public void ChangeIcon()
        {
            IconString = IsRunning ? "oi oi-media-pause" : "oi oi-media-play";
        }

        /// <summary>
        /// Disposes the timer service and unsubscribes from events.
        /// </summary>
        public void Dispose()
        {
            OnTimerChanged -= async (o, e) => await CalculateDisplayValue();
        }
    }
}
