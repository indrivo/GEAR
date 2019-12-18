using System;
using System.Threading;
using System.Threading.Tasks;

namespace GR.Core.Abstractions
{
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// Add new task
        /// </summary>
        /// <param name="workItem"></param>
        void PushBackgroundWorkItemInQueue(Func<CancellationToken, Task> workItem);

        /// <summary>
        /// Remove work item
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Func<CancellationToken, Task>> RemoveBackgroundWorkItemFromQueueAsync(
            CancellationToken cancellationToken);
    }
}
