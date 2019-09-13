using System;
using System.Threading;
using System.Threading.Tasks;

namespace ST.Core.Abstractions
{
    public interface IBackgroundTaskQueue
    {
        void PushBackgroundWorkItemInQueue(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> RemoveBackgroundWorkItemFromQueueAsync(
            CancellationToken cancellationToken);
    }
}
