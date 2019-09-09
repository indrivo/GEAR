using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ST.Core.Helpers;
using ST.TaskManager.Abstractions.Models.ViewModels;

namespace ST.TaskManager.Abstractions
{
    public interface ITaskManager
    {
        /// <summary>
        /// Get task by task Id
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<ResultModel<GetTaskViewModel>> GetTaskAsync(Guid taskId);

        /// <summary>
        /// Get task items by task Id
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<ResultModel<List<GetTaskItemViewModel>>> GetTaskItemsAsync(Guid taskId);

        /// <summary>
        /// Get tasks list by author user id
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="deleted"></param>
        /// <returns></returns>
        Task<ResultModel<List<GetTaskViewModel>>> GetUserTasksAsync(string userName, bool deleted);

        /// <summary>
        /// Get tasks list by assigner id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ResultModel<List<GetTaskViewModel>>> GetAssignedTasksAsync(Guid userId);

        /// <summary>
        /// Create task with task items
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> CreateTaskAsync(CreateTaskViewModel task);

        /// <summary>
        /// Update task (without task items)
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateTaskAsync(UpdateTaskViewModel task);

        /// <summary>
        /// Delete task logical
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteTaskAsync(Guid taskId);

        /// <summary>
        /// Delete task permanent
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<ResultModel> DeletePermanentTaskAsync(Guid taskId);

        /// <summary>
        /// Restore task from deleted status
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<ResultModel> RestoreTaskAsync(Guid taskId);

        /// <summary>
        /// Add task item to an existing task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> CreateTaskItemAsync(CreateTaskItemViewModel task);

        /// <summary>
        /// Update task item
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> UpdateTaskItemAsync(UpdateTaskItemViewModel task);

        /// <summary>
        /// Delete task item permanent
        /// </summary>
        /// <param name="taskItemId"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteTaskItemAsync(Guid taskItemId);
    }
}