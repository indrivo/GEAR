using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.TaskManager.Abstractions;
using ST.TaskManager.Abstractions.Helpers;
using ST.TaskManager.Abstractions.Models;
using ST.TaskManager.Abstractions.Models.ViewModels;
using ST.TaskManager.Data;

namespace ST.TaskManager
{
    public class TaskManager<TContext> : ITaskManager where TContext : TaskManagerDbContext, ITaskManagerContext
    {
        private readonly TContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public TaskManager(TContext context)
        {
            _context = context;
        }

        public async Task<ResultModel<GetTaskViewModel>> GetTaskAsync(Guid taskId)
        {
            var dbTaskResult = await _context.Tasks.FirstOrDefaultAsync(x => (x.Id == taskId) & (x.IsDeleted == false));
            if (dbTaskResult == null)
                return ExceptionHandler.ReturnErrorModel<GetTaskViewModel>(ExceptionMessagesEnum.TaskNotFound);

            var dto = GetTaskMapper(dbTaskResult);

            return new ResultModel<GetTaskViewModel>
            {
                IsSuccess = true,
                Result = dto
            };
        }

        public async Task<ResultModel<List<TaskItemViewModel>>> GetTaskItemsAsync(Guid taskId)
        {
            var dbTaskItemsResult = await _context.Tasks.FirstOrDefaultAsync(x => (x.Id == taskId) & (x.IsDeleted == false));
            var dto = new List<TaskItemViewModel>();
            if (dbTaskItemsResult == null)
                return ExceptionHandler.ReturnErrorModel<List<TaskItemViewModel>>(ExceptionMessagesEnum.TaskNotFound);

            if (dbTaskItemsResult.TaskItems.Count > 0)
                dto.AddRange(TaskItemsMapper(dbTaskItemsResult));

            return new ResultModel<List<TaskItemViewModel>>
            {
                IsSuccess = true,
                Result = dto
            };
        }

        public async Task<ResultModel<List<GetTaskViewModel>>> GetTasksAsync(Guid userId)
        {
            var dbTasksResult = await _context.Tasks.Where(x => (x.UserId == userId) & (x.IsDeleted == false)).ToListAsync();
            var taskList = new List<GetTaskViewModel>();

            if (dbTasksResult.Count > 0)
                taskList.AddRange(dbTasksResult.Select(GetTaskMapper));
            else
                return ExceptionHandler.ReturnErrorModel<List<GetTaskViewModel>>(ExceptionMessagesEnum.TaskNotFound);

            return new ResultModel<List<GetTaskViewModel>>
            {
                IsSuccess = true,
                Result = taskList
            };
        }

        public async Task<ResultModel<Guid>> CreateTaskAsync(CreateTaskViewModel task)
        {
            var taskModel = CreateTaskMapper(task);

            _context.Tasks.Add(taskModel);
            var result = await _context.SaveAsync();

            return new ResultModel<Guid>
            {
                IsSuccess = result.IsSuccess,
                Result = taskModel.Id
            };
        }

        public async Task<ResultModel> UpdateTaskAsync(UpdateTaskViewModel task)
        {
            var taskModel = UpdateTaskMapper(task);

            _context.Tasks.Add(taskModel);
            var result = await _context.SaveAsync();

            return new ResultModel
            {
                IsSuccess = result.IsSuccess,
            };
        }

        public async Task<ResultModel> DeleteTaskAsync(Guid taskId)
        {
            var file = _context.Tasks.FirstOrDefault(x => x.Id == taskId);
            if (file == null) return ExceptionHandler.ReturnErrorModel(ExceptionMessagesEnum.TaskNotFound);

            file.IsDeleted = true;
            _context.Tasks.Update(file);
            var result = await _context.SaveAsync();


            return new ResultModel
            {
                IsSuccess = result.IsSuccess
            };
        }

        public async Task<ResultModel> DeletePermanentTaskAsync(Guid taskId)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.Id == taskId);
            if (task == null) return ExceptionHandler.ReturnErrorModel(ExceptionMessagesEnum.TaskNotFound);

            _context.Tasks.Remove(task);

            var result = await _context.SaveAsync();

            return new ResultModel
            {
                IsSuccess = result.IsSuccess
            };
        }

        public async Task<ResultModel<Guid>> CreateTaskItemAsync(TaskItemViewModel task)
        {
            var dbTaskResult = _context.Tasks.FirstOrDefault(x => (x.Id == task.TaskId));
            var taskModel = new TaskItem { Name = task.Name,Task = dbTaskResult};

            _context.TaskItems.Add(taskModel);
            var result = await _context.SaveAsync();

            return new ResultModel<Guid>
            {
                IsSuccess = result.IsSuccess,
                Result = taskModel.Id
            };
        }

        public async Task<ResultModel<Guid>> UpdateTaskItemAsync(TaskItemViewModel task)
        {
            var dbTaskResult = _context.TaskItems.FirstOrDefault(x => (x.Id == task.Id));
            if (dbTaskResult != null)
            {
                dbTaskResult.Name = task.Name;
                dbTaskResult.IsDone = task.IsDone;
                _context.TaskItems.Add(dbTaskResult);
            }
            else
            {
                return ExceptionHandler.ReturnErrorModel<Guid>(ExceptionMessagesEnum.TaskNotFound);
            }

            var result = await _context.SaveAsync();

            return new ResultModel<Guid>
            {
                IsSuccess = result.IsSuccess,
                Result = dbTaskResult.Id
            };
        }

        public async Task<ResultModel> DeleteTaskItemAsync(Guid taskItemId)
        {
            var task = _context.TaskItems.FirstOrDefault(x => x.Id == taskItemId);
            if (task == null) return ExceptionHandler.ReturnErrorModel(ExceptionMessagesEnum.TaskNotFound);

            _context.TaskItems.Remove(task);

            var result = await _context.SaveAsync();

            return new ResultModel
            {
                IsSuccess = result.IsSuccess
            };
        }

        private Abstractions.Models.Task UpdateTaskMapper(UpdateTaskViewModel taskViewModel)
        {
            var dbTaskResult = _context.Tasks.FirstOrDefault(x => (x.Id == taskViewModel.Id) & (x.IsDeleted == false));

            if (dbTaskResult == null) return null;

            dbTaskResult.Name = taskViewModel.Name;
            dbTaskResult.Description = taskViewModel.Description;
            dbTaskResult.StartDate = taskViewModel.StartDate;
            dbTaskResult.EndDate = taskViewModel.EndDate;
            dbTaskResult.Status = taskViewModel.Status;
            dbTaskResult.TaskPriority = taskViewModel.TaskPriority;

            return dbTaskResult;
        }

        private static Abstractions.Models.Task CreateTaskMapper(CreateTaskViewModel taskViewModel)
        {
            var dto = new Abstractions.Models.Task
            {
                Name = taskViewModel.Name,
                Description = taskViewModel.Description,
                StartDate = taskViewModel.StartDate,
                EndDate = taskViewModel.EndDate,
                Status = taskViewModel.Status,
                TaskPriority = taskViewModel.TaskPriority
            };
            if (taskViewModel.TaskItems == null) return dto;

            foreach (var item in taskViewModel.TaskItems) dto.TaskItems.Add(new TaskItem { IsDone = item.IsDone, Name = item.Name });


            return dto;
        }

        private static GetTaskViewModel GetTaskMapper(Abstractions.Models.Task dbTaskResult)
        {
            var dto = new GetTaskViewModel
            {
                Id = dbTaskResult.Id,
                Name = dbTaskResult.Name,
                Description = dbTaskResult.Description,
                StartDate = dbTaskResult.StartDate,
                EndDate = dbTaskResult.EndDate,
                Status = dbTaskResult.Status,
                TaskPriority = dbTaskResult.TaskPriority
            };
            if (dbTaskResult.TaskItems?.Count > 0)
                dto.TaskItems.AddRange(TaskItemsMapper(dbTaskResult));
            return dto;
        }

        private static IEnumerable<TaskItemViewModel> TaskItemsMapper(Abstractions.Models.Task dbTaskResult)
        {
            return dbTaskResult.TaskItems.Select(item => new TaskItemViewModel {Id = item.Id, IsDone = item.IsDone, Name = item.Name}).ToList();
        }
    }
}
