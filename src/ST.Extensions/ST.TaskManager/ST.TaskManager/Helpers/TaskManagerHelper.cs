using System;
using System.Collections.Generic;
using System.Linq;
using Mapster;
using Newtonsoft.Json;
using ST.Core;
using ST.Core.Helpers;
using ST.TaskManager.Abstractions.Models;
using ST.TaskManager.Abstractions.Models.ViewModels;

namespace ST.TaskManager.Helpers
{
    public class TaskManagerHelper
    {
        internal static Task TaskMapper(UpdateTaskViewModel taskViewModel, Task dbTaskResult)
        {
            if (dbTaskResult == null) return null;

            dbTaskResult.Name = taskViewModel.Name;
            dbTaskResult.Description = taskViewModel.Description;
            dbTaskResult.StartDate = taskViewModel.StartDate;
            dbTaskResult.EndDate = taskViewModel.EndDate;
            dbTaskResult.Status = taskViewModel.Status;
            dbTaskResult.TaskPriority = taskViewModel.TaskPriority;
            dbTaskResult.UserId = taskViewModel.UserId;
            dbTaskResult.Files = JsonConvert.SerializeObject(taskViewModel.Files);

            return dbTaskResult;
        }

        internal static Task TaskMapper(CreateTaskViewModel taskViewModel)
        {
            var dto = taskViewModel.Adapt<Task>();
            dto.Files = JsonConvert.SerializeObject(taskViewModel.Files);
            if (taskViewModel.TaskItems == null) return dto;

            foreach (var item in taskViewModel.TaskItems)
                dto.TaskItems.Add(
                    new TaskItem
                    {
                        IsDone = false,
                        Name = item.Name
                    });

            return dto;
        }

        internal static GetTaskViewModel GetTaskMapper(Task dbTaskResult)
        {
            var dto = dbTaskResult.Adapt<GetTaskViewModel>();
            dto.Files = JsonConvert.DeserializeObject<List<Guid>>(dbTaskResult.Files);
            dto.TaskItemsCount = CountTaskItems(dbTaskResult);
            return dto;
        }

        internal static IEnumerable<GetTaskItemViewModel> TaskItemsMapper(Task dbTaskResult)
        {
            return dbTaskResult.TaskItems.Select(item => new GetTaskItemViewModel
            {
                Id = item.Id,
                IsDone = item.IsDone,
                Name = item.Name,
            }).AsEnumerable();
        }

        internal static ResultModel<PagedResult<GetTaskViewModel>> GetTasksAsync(PagedResult<Task> dbTasksResult)
        {
            var taskPage = new PagedResult<GetTaskViewModel>
            {
                CurrentPage = dbTasksResult.CurrentPage,
                PageCount = dbTasksResult.PageCount,
                RowCount = dbTasksResult.RowCount,
                PageSize = dbTasksResult.PageSize
            };

            if (dbTasksResult.Results.Count > 0)
                for (var index = 0; index < dbTasksResult.Results.Count; index++)
                {
                    var item = dbTasksResult.Results[index];
                    taskPage.Results.Add(GetTaskMapper(item));
                }

            return new ResultModel<PagedResult<GetTaskViewModel>>
            {
                IsSuccess = true,
                Result = taskPage
            };
        }

        private static int[] CountTaskItems(Task dbTasksResult)
        {
            if (dbTasksResult.TaskItems == null || dbTasksResult.TaskItems.Count == 0) return new[] {0, 0};

            var total = dbTasksResult.TaskItems.Count;
            var completed = dbTasksResult.TaskItems.Count(x => x.IsDone == true);

            return new[] {completed, total};
        }
    }
}
