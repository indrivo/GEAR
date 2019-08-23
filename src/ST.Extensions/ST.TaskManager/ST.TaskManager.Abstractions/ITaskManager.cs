using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ST.Core.Helpers;
using ST.TaskManager.Abstractions.Models.ViewModels;

namespace ST.TaskManager.Abstractions
{
    public interface ITaskManager //<in TUser> where TUser : IdentityUser
    {
        Task<ResultModel<GetTaskViewModel>> GetTaskAsync(Guid taskId);
        Task<ResultModel<List<TaskItemViewModel>>> GetTaskItemsAsync(Guid taskId);
        Task<ResultModel<List<GetTaskViewModel>>> GetTasksAsync(Guid userId);
        Task<ResultModel<Guid>> CreateTaskAsync(CreateTaskViewModel task);
        Task<ResultModel> UpdateTaskAsync(UpdateTaskViewModel task);
        Task<ResultModel> DeleteTaskAsync(Guid taskId);
        Task<ResultModel> DeletePermanentTaskAsync(Guid taskId);
        Task<ResultModel<Guid>> CreateTaskItemAsync(TaskItemViewModel task);
        Task<ResultModel<Guid>> UpdateTaskItemAsync(TaskItemViewModel task);
        Task<ResultModel> DeleteTaskItemAsync(Guid taskItemId);
    }
}