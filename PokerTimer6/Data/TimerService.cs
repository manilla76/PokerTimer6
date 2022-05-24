using Microsoft.AspNetCore.Components;
using PokerTimer6.Models;

namespace PokerTimer6.Data
{
    public class TimerService
    {
        private static event EventHandler<TimerEventArgs>? OnTimerChanged;
        
        private static TimeSpan timeRemaining { get; set; }

        private bool isRunning = false;
        public bool TimerEmpty { get => timeRemaining.Seconds <= 0; }
        public bool IsRunning { get => isRunning; set { isRunning = value; NotifyDataChanged(); } }

        public bool SoundAlert { get => timeRemaining.TotalSeconds <= 0 & !isRunning;}

        private static Timer InternalTimer = new Timer((state) =>
        {
            timeRemaining = timeRemaining.Subtract(new TimeSpan(0, 0, 0, 1));
            if (OnTimerChanged is not null)
            {
                OnTimerChanged.Invoke(null, new TimerEventArgs(timeRemaining));
            }
        });

        public event Action OnChange;

        private void NotifyDataChanged() => OnChange?.Invoke();

        private string? displayValue;


        public string? DisplayValue { 
            get => displayValue;
            set { displayValue = value; NotifyDataChanged(); } 
        }

        public Task StartPauseAsync()
        {
            IsRunning = !IsRunning;
            if (IsRunning & timeRemaining.TotalSeconds > 0)
                InternalTimer.Change(1000, 1000);
            else
                StopAsync();         

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            IsRunning = false;
            InternalTimer.Change(Timeout.Infinite, Timeout.Infinite);
            NotifyDataChanged();
            return Task.CompletedTask;
        }

        public TimerService()
        {
            OnTimerChanged += async (o, e) =>
            {
                // Since we're not necessarily on the thread that has proper access to the renderer context
                // we need to use the InvokeAsync() method, which takes care of running our code on the right thread.
                CalculateDisplayValue();                
            };

            // Calculate the initial value for the timer.
            CalculateDisplayValue();
            
        }
        public void SetTimeRemaining()
        {
            timeRemaining = new TimeSpan(0, 0, 0, 10);
            CalculateDisplayValue();
        }

        public void SetTimeRemaining(TimeSpan time)
        {
            timeRemaining = time;
            CalculateDisplayValue();
        }
        private void CalculateDisplayValue()
        {   
            this.DisplayValue = $"{timeRemaining.Minutes}:{timeRemaining.Seconds.ToString("00")}";
            if (timeRemaining.TotalSeconds <= 0)
                StopAsync();
            
        }


    }
}
