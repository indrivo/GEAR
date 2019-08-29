using System.Collections.Generic;
using System.Linq;
using ST.Core.Helpers;
using ST.TaskManager.Abstractions.Models;
using ST.TaskManager.Abstractions.Models.ViewModels;

namespace ST.TaskManager
{
    public class TaskManagerHelper
    {
        internal static Task UpdateTaskMapper(UpdateTaskViewModel taskViewModel,Task dbTaskResult)
        {
            if (dbTaskResult == null) return null;

            dbTaskResult.Name = taskViewModel.Name;
            dbTaskResult.Description = taskViewModel.Description;
            dbTaskResult.StartDate = taskViewModel.StartDate;
            dbTaskResult.EndDate = taskViewModel.EndDate;
            dbTaskResult.Status = taskViewModel.Status;
            dbTaskResult.TaskPriority = taskViewModel.TaskPriority;
            dbTaskResult.UserId = taskViewModel.UserId;

            return dbTaskResult;
        }

        internal static Task CreateTaskMapper(CreateTaskViewModel taskViewModel)
        {
            var dto = new Task
            {
                Name = taskViewModel.Name,
                Description = taskViewModel.Description,
                StartDate = taskViewModel.StartDate,
                EndDate = taskViewModel.EndDate,
                Status = taskViewModel.Status,
                UserId = taskViewModel.UserId,
                TaskPriority = taskViewModel.TaskPriority,
            };
            if (taskViewModel.TaskItems == null) return dto;

            foreach (var item in taskViewModel.TaskItems) dto.TaskItems.Add(
                new TaskItem
                {
                    IsDone = false,
                    Name = item.Name
                });

            return dto;
        }

        internal static GetTaskViewModel GetTaskMapper(Task dbTaskResult)
        {
            var dto = new GetTaskViewModel
            {
                Id = dbTaskResult.Id,
                Name = dbTaskResult.Name,
                Description = dbTaskResult.Description,
                StartDate = dbTaskResult.StartDate,
                EndDate = dbTaskResult.EndDate,
                Status = dbTaskResult.Status,
                UserId = dbTaskResult.UserId,
                TaskPriority = dbTaskResult.TaskPriority
            };
            return dto;
        }

        internal static IEnumerable<TaskItemViewModel> TaskItemsMapper(Task dbTaskResult)
        {
            return dbTaskResult.TaskItems.Select(item => new TaskItemViewModel
            {
                Id = item.Id,
                IsDone = item.IsDone,
                Name = item.Name,
                TaskId = item.Task.Id
            }).AsEnumerable();
        }

        internal static ResultModel<List<GetTaskViewModel>> GetTasksAsync(IReadOnlyCollection<Task> dbTasksResult)
        {
            var taskList = new List<GetTaskViewModel>();
            if (dbTasksResult.Count > 0)
                taskList.AddRange(dbTasksResult.Select(GetTaskMapper));

            return new ResultModel<List<GetTaskViewModel>>
            {
                IsSuccess = true,
                Result = taskList
            };
        }
    }
}
