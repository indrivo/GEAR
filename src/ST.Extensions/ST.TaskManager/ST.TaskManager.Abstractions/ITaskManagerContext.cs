using Microsoft.EntityFrameworkCore;
using ST.Core.Abstractions;
using ST.TaskManager.Abstractions.Models;

namespace ST.TaskManager.Abstractions
{
    public interface ITaskManagerContext : IDbContext
    {
        /// <summary>
        /// Tasks
        /// </summary>
        DbSet<Task> Tasks { get; set; }

        /// <summary>
        /// Task items
        /// </summary>
        DbSet<TaskItem> TaskItems { get; set; }
    }
}
