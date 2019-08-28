using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.TaskManager.Abstractions;
using ST.TaskManager.Abstractions.Helpers;
using ST.TaskManager.Abstractions.Models;
using ST.TaskManager.Abstractions.Models.ViewModels;
using Task = ST.TaskManager.Abstractions.Models.Task;

namespace ST.TaskManager
{
    public class TaskManager : TaskManagerHelper, ITaskManager
    {
        private readonly ITaskManagerContext _context;

        public TaskManager(ITaskManagerContext context)
        {
            _context = context;
        }

        #region Task GET

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
            var task = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == taskId);
            if (task == null) return ExceptionHandler.ReturnErrorModel<List<TaskItemViewModel>>(ExceptionMessagesEnum.TaskNotFound);

            var dbTaskItemsResult = await _context.TaskItems.Where(x => x.Task == task).ToListAsync();
            var dto = new List<TaskItemViewModel>();
            if (dbTaskItemsResult.Any())
                dto.AddRange(TaskItemsMapper(new Task {TaskItems = dbTaskItemsResult}));
            else
                return ExceptionHandler.ReturnErrorModel<List<TaskItemViewModel>>(ExceptionMessagesEnum.TaskNotFound);

            return new ResultModel<List<TaskItemViewModel>>
            {
                IsSuccess = true,
                Result = dto
            };
        }

        public async Task<ResultModel<List<GetTaskViewModel>>> GetUserTasksAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return ExceptionHandler.ReturnErrorModel<List<GetTaskViewModel>>(ExceptionMessagesEnum.NullParameter);

            var dbTasksResult = await _context.Tasks.Where(x => (x.Author == userName.Trim()) & (x.IsDeleted == false)).ToListAsync();
            return GetTasksAsync(dbTasksResult);
        }

        public async Task<ResultModel<List<GetTaskViewModel>>> GetAssignedTasksAsync(Guid userId)
        {
            if (userId == Guid.Empty) return ExceptionHandler.ReturnErrorModel<List<GetTaskViewModel>>(ExceptionMessagesEnum.NullParameter);

            var dbTasksResult = await _context.Tasks.Where(x => (x.UserId == userId) & (x.IsDeleted == false)).ToListAsync();
            return GetTasksAsync(dbTasksResult);
        }

        #endregion

        #region Task

        public async Task<ResultModel<Guid>> CreateTaskAsync(CreateTaskViewModel task)
        {

            var taskModel = CreateTaskMapper(task);

            _context.Tasks.Add(taskModel);
            var result = await _context.SaveDependenceAsync();

            return new ResultModel<Guid>
            {
                IsSuccess = result.IsSuccess,
                Result = taskModel.Id,
                Errors = result.Errors
            };
        }

        public async Task<ResultModel> UpdateTaskAsync(UpdateTaskViewModel task)
        {
            var dbTaskResult = _context.Tasks.FirstOrDefault(x => (x.Id == task.Id) & (x.IsDeleted == false));
            if (dbTaskResult == null)
                return ExceptionHandler.ReturnErrorModel(ExceptionMessagesEnum.TaskNotFound);

            var taskModel = UpdateTaskMapper(task, dbTaskResult);
            _context.Tasks.Update(taskModel);
            var result = await _context.SaveDependenceAsync();

            return new ResultModel
            {
                IsSuccess = result.IsSuccess,
                Errors = result.Errors
            };
        }

        public async Task<ResultModel> DeleteTaskAsync(Guid taskId)
        {
            var file = _context.Tasks.FirstOrDefault(x => x.Id == taskId);
            if (file == null) return ExceptionHandler.ReturnErrorModel(ExceptionMessagesEnum.TaskNotFound);

            file.IsDeleted = true;
            _context.Tasks.Update(file);
            var result = await _context.SaveDependenceAsync();

            return new ResultModel
            {
                IsSuccess = result.IsSuccess,
                Errors = result.Errors
            };
        }

        public async Task<ResultModel> DeletePermanentTaskAsync(Guid taskId)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.Id == taskId);
            if (task == null) return ExceptionHandler.ReturnErrorModel(ExceptionMessagesEnum.TaskNotFound);

            _context.Tasks.Remove(task);

            var result = await _context.SaveDependenceAsync();

            return new ResultModel
            {
                IsSuccess = result.IsSuccess,
                Errors = result.Errors
            };
        }

        #endregion

        #region Task Items

        public async Task<ResultModel<Guid>> CreateTaskItemAsync(TaskItemViewModel task)
        {
            var dbTaskResult = _context.Tasks.FirstOrDefault(x => (x.Id == task.TaskId));
            var taskModel = new TaskItem {Name = task.Name, Task = dbTaskResult};

            _context.TaskItems.Add(taskModel);
            var result = await _context.SaveDependenceAsync();

            return new ResultModel<Guid>
            {
                IsSuccess = result.IsSuccess,
                Result = taskModel.Id,
                Errors = result.Errors
            };
        }

        public async Task<ResultModel<Guid>> UpdateTaskItemAsync(TaskItemViewModel task)
        {
            var dbTaskResult = _context.TaskItems.FirstOrDefault(x => (x.Id == task.Id));
            if (dbTaskResult != null)
            {
                dbTaskResult.Name = task.Name;
                dbTaskResult.IsDone = task.IsDone;
                _context.TaskItems.Update(dbTaskResult);
            }
            else
            {
                return ExceptionHandler.ReturnErrorModel<Guid>(ExceptionMessagesEnum.TaskNotFound);
            }

            var result = await _context.SaveDependenceAsync();

            return new ResultModel<Guid>
            {
                IsSuccess = result.IsSuccess,
                Result = dbTaskResult.Id,
                Errors = result.Errors
            };
        }

        public async Task<ResultModel> DeleteTaskItemAsync(Guid taskItemId)
        {
            var task = _context.TaskItems.FirstOrDefault(x => x.Id == taskItemId);
            if (task == null) return ExceptionHandler.ReturnErrorModel(ExceptionMessagesEnum.TaskNotFound);

            _context.TaskItems.Remove(task);

            var result = await _context.SaveDependenceAsync();

            return new ResultModel
            {
                IsSuccess = result.IsSuccess,
                Errors = result.Errors
            };
        }

        #endregion
    }
}
