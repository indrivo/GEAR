using System;
using System.Threading;
using System.Threading.Tasks;

namespace ST.Core.Abstractions
{
    public interface IBackgroundTaskQueue
    {
        void PushQueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueAsync(
            CancellationToken cancellationToken);
    }
}
