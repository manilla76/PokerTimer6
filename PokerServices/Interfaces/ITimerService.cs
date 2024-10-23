using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServices.Interfaces
{
    public interface ITimerService
    {
        Timer TTimer { get; }
        TimeSpan OriginalTimeSpan { get; }
        TimeSpan TimeRemaining { get; }
        bool IsRunning { get; }

        void StartTimer();
        void StopTimer();
        void ResetTimer();
        void SetTime(TimeSpan time);
        void Dispose();
        void SetTime(double minutes);
    }
}
