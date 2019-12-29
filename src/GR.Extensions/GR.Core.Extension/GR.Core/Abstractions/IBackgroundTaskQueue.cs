using System;
using System.Threading;
using System.Threading.Tasks;

namespace GR.Core.Abstractions
{
    public interface IBackgroundTaskQueue
    {
        void PushBackgroundWorkItemInQueue(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> RemoveBackgroundWorkItemFromQueueAsync(
            CancellationToken cancellationToken);
    }
}
