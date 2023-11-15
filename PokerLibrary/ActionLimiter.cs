using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerLibrary
{
    public sealed class ActionLimiter
    {
        //public Task<bool> QueueAsync();
        public static ActionLimiter Create(Func<Task> toRun, int backOffPeriod) 
            => new ActionLimiter(toRun, backOffPeriod > 300 ? backOffPeriod : 300);
        private int backOffPeriod = 0;
        private Func<Task> taskToRun;
        private Task activeTask = Task.CompletedTask;
        private TaskCompletionSource<bool>? queuedTaskCompletionSource;
        private TaskCompletionSource<bool>? activeTaskCompletionSource;

        private async Task RunQueueAsync()
        {
            if (activeTaskCompletionSource is not null && activeTaskCompletionSource.Task.IsCompleted)
            {
                activeTaskCompletionSource = null;
            }
            if (activeTaskCompletionSource is not null)
            {
                return;
            }
            while (queuedTaskCompletionSource is not null)
            {
                activeTaskCompletionSource = queuedTaskCompletionSource;
                queuedTaskCompletionSource = null;
                var backoffTask = Task.Delay(backOffPeriod);
                var mainTask = taskToRun.Invoke();
                await Task.WhenAll(new Task[] { mainTask, backoffTask });
                activeTaskCompletionSource.TrySetResult(true);
                activeTaskCompletionSource = null;
            }
            return;
        }
        private ActionLimiter(Func<Task> toRun, int backOffPeriod)
        {
            this.backOffPeriod = backOffPeriod;
            taskToRun = toRun;
        }

        public Task<bool> QueueAsync()
        {
            var oldCompletionTask = queuedTaskCompletionSource;
            var newCompletionTask = new TaskCompletionSource<bool>();
            var task = newCompletionTask.Task;
            queuedTaskCompletionSource = newCompletionTask;
            if (oldCompletionTask is not null && !oldCompletionTask.Task.IsCompleted)
            {
                oldCompletionTask?.TrySetResult(false);
            }
            if (activeTask is null || activeTask.IsCompleted)
            {
                activeTask = this.RunQueueAsync();
            }
            return task;
        }
    }
}
