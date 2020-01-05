using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using GR.Core.Abstractions;

namespace GR.Core.Services
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _workItems =
            new ConcurrentQueue<Func<CancellationToken, Task>>();

        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        /// <summary>
        /// Start task on background
        /// </summary>
        /// <param name="workItem"></param>
        public void PushBackgroundWorkItemInQueue(
            Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        /// <summary>
        /// Remove task from background
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Func<CancellationToken, Task>> RemoveBackgroundWorkItemFromQueueAsync(
            CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }

        /// <summary>
        /// Work items on pending
        /// </summary>
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _workItemsOnPending =
            new ConcurrentQueue<Func<CancellationToken, Task>>();

        /// <summary>
        /// Push new item to be executed after install
        /// </summary>
        /// <param name="workItem"></param>
        public void PushBackgroundWorkItemInQueueToBeExecutedAfterAppInstalled(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItemsOnPending.Enqueue(workItem);
        }

        /// <summary>
        /// Add to execute pending background items
        /// </summary>
        public void AddToExecutePendingBackgroundWorkItems()
        {
            for (var i = 0; i < _workItemsOnPending.Count; i++)
            {
                _workItemsOnPending.TryDequeue(out var taskToExecute);
                PushBackgroundWorkItemInQueue(taskToExecute);
            }
        }
    }
}
