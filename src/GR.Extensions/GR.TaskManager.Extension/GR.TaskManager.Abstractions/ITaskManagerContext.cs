using Microsoft.EntityFrameworkCore;
using GR.Core.Abstractions;
using GR.TaskManager.Abstractions.Models;

namespace GR.TaskManager.Abstractions
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

        /// <summary>
        /// Task assigned users
        /// </summary>
        DbSet<TaskAssignedUser> TaskAssignedUsers { get; set; }
    }
}
