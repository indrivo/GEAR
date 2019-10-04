using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ST.Core;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Core.Helpers.Pagination;
using ST.Identity.Abstractions;
using ST.TaskManager.Abstractions;
using ST.TaskManager.Abstractions.Helpers;
using ST.TaskManager.Abstractions.Models;
using ST.TaskManager.Abstractions.Models.ViewModels;
using ST.TaskManager.Helpers;
using Task = ST.TaskManager.Abstractions.Models.Task;

namespace ST.TaskManager.Services
{
    public class TaskManager : TaskManagerHelper, ITaskManager
    {
        #region Injectable
        /// <summary>
        /// Inject db context
        /// </summary>
        private readonly ITaskManagerContext _context;

        /// <summary>
        /// Inject notification service
        /// </summary>
        private readonly TaskManagerNotificationService _notify;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<ApplicationUser> _userManager;
        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="identity"></param>
        /// <param name="userManager"></param>
        public TaskManager(ITaskManagerContext context, IUserManager<ApplicationUser> identity, IUserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _notify = new TaskManagerNotificationService(identity);
        }

        #region Task GET

        public async Task<ResultModel<GetTaskViewModel>> GetTaskAsync(Guid taskId)
        {
            if (taskId == Guid.Empty) return ExceptionMessagesEnum.NullParameter.ToErrorModel<GetTaskViewModel>();

            var dbTaskResult = await _context.Tasks
                .Include(x => x.AssignedUsers)
                .Include(x => x.TaskItems)
                .FirstOrDefaultAsync(x => x.Id == taskId);
            if (dbTaskResult == null)
                return ExceptionMessagesEnum.TaskNotFound.ToErrorModel<GetTaskViewModel>();
            var currentUser = (await _userManager.GetCurrentUserAsync()).Result?.Id.ToGuid();
            var dto = GetTaskMapper(dbTaskResult, currentUser);

            return new ResultModel<GetTaskViewModel>
            {
                IsSuccess = true,
                Result = dto
            };
        }

        public async Task<ResultModel<List<GetTaskItemViewModel>>> GetTaskItemsAsync(Guid taskId)
        {
            if (taskId == Guid.Empty) return ExceptionMessagesEnum.NullParameter.ToErrorModel<List<GetTaskItemViewModel>>();

            var task = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == taskId);
            if (task == null) return ExceptionMessagesEnum.TaskNotFound.ToErrorModel<List<GetTaskItemViewModel>>();

            var dbTaskItemsResult = await _context.TaskItems.Where(x => x.Task == task).ToListAsync();
            var dto = new List<GetTaskItemViewModel>();
            if (dbTaskItemsResult.Any())
                dto.AddRange(TaskItemsMapper(new Task { TaskItems = dbTaskItemsResult }));
            else
                return ExceptionMessagesEnum.TaskItemsNotFound.ToErrorModel<List<GetTaskItemViewModel>>();

            return new ResultModel<List<GetTaskItemViewModel>>
            {
                IsSuccess = true,
                Result = dto
            };
        }

        public async Task<ResultModel<PagedResult<GetTaskViewModel>>> GetUserTasksAsync(string userName, PageRequest request)
        {
            if (string.IsNullOrEmpty(userName)) return ExceptionMessagesEnum.NullParameter.ToErrorModel<PagedResult<GetTaskViewModel>>();

            var dbTasksResult = await _context.Tasks
                .Include(x => x.AssignedUsers)
                .Include(x => x.TaskItems)
                .Where(x => (x.Author == userName.Trim()) & (x.IsDeleted == request.Deleted))
                .OrderByWithDirection(x => TypeHelper.GetPropertyValue(x, request.Attribute), request.Descending)
                .GetPagedAsync(request.Page, request.PageSize);
            var currentUser = (await _userManager.GetCurrentUserAsync()).Result?.Id.ToGuid();
            return GetTasksAsync(dbTasksResult, currentUser);
        }

        public async Task<ResultModel<PagedResult<GetTaskViewModel>>> GetAssignedTasksAsync(Guid userId, string userName, PageRequest request)
        {
            if (userId == Guid.Empty) return ExceptionMessagesEnum.NullParameter.ToErrorModel<PagedResult<GetTaskViewModel>>();

            var dbTasksResult = await _context.Tasks
                .Include(x => x.AssignedUsers)
                .Where(x => (x.UserId == userId || x.AssignedUsers.Any(c => c.UserId.Equals(userId)))
                            & (x.IsDeleted == request.Deleted)
                            & (x.Author != userName))
                .OrderByWithDirection(x => TypeHelper.GetPropertyValue(x, request.Attribute), request.Descending)
                .GetPagedAsync(request.Page, request.PageSize);

            return GetTasksAsync(dbTasksResult, userId);
        }

        #endregion

        #region Task

        public virtual async Task<ResultModel<Guid>> CreateTaskAsync(CreateTaskViewModel task)
        {
            var taskModel = TaskMapper(task);
            taskModel.TaskNumber = await GenerateTaskNumberAsync();
            foreach (var user in task.UserTeam)
            {
                var checkUser = await _userManager
                        .UserManager
                        .Users
                        .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id.ToGuid().Equals(user));
                if (checkUser == null) continue;
                taskModel.AssignedUsers.Add(new TaskAssignedUser
                {
                    UserId = user
                });
            }
            _context.Tasks.Add(taskModel);
            var result = await _context.PushAsync();

            if (result.IsSuccess) await _notify.AddTaskNotificationAsync(taskModel);
            return new ResultModel<Guid>
            {
                IsSuccess = result.IsSuccess,
                Result = taskModel.Id,
                Errors = result.Errors
            };
        }

        public virtual async Task<ResultModel> UpdateTaskAsync(UpdateTaskViewModel task)
        {
            var dbTaskResult = _context.Tasks
                .Include(x => x.AssignedUsers)
                .FirstOrDefault(x => (x.Id == task.Id) & (x.IsDeleted == false));


            if (dbTaskResult == null)
                return ExceptionMessagesEnum.TaskNotFound.ToErrorModel();

            var taskModel = TaskMapper(task, dbTaskResult);
            _context.Tasks.Update(taskModel);
            var result = await _context.PushAsync();

            if (result.IsSuccess)
            {
                await AddOrUpdateUsersToTaskGroupAsync(dbTaskResult, task.UserTeam);
                await _notify.UpdateTaskNotificationAsync(taskModel);
            }

            return new ResultModel
            {
                IsSuccess = result.IsSuccess,
                Errors = result.Errors
            };
        }

        /// <summary>
        /// Add or remove user to task team
        /// </summary>
        /// <param name="task"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AddOrUpdateUsersToTaskGroupAsync(Task task, IEnumerable<Guid> users)
        {
            var response = new ResultModel();
            if (task == null)
            {
                response.Errors.Add(new ErrorModel(string.Empty, nameof(NullReferenceException)));
                return response;
            }

            var current = task.AssignedUsers?.ToList() ?? new List<TaskAssignedUser>();
            _context.TaskAssignedUsers.RemoveRange(current);
            var newUsers = users.Select(x => new TaskAssignedUser
            {
                UserId = x,
                TaskId = task.Id
            });

            await _context.TaskAssignedUsers.AddRangeAsync(newUsers);

            return await _context.PushAsync();
        }

        public async Task<ResultModel> DeleteTaskAsync(Guid taskId)
        {
            if (taskId == Guid.Empty) return ExceptionMessagesEnum.NullParameter.ToErrorModel();

            var dbTaskResult = _context.Tasks.FirstOrDefault(x => x.Id == taskId);
            if (dbTaskResult == null) return ExceptionMessagesEnum.TaskNotFound.ToErrorModel();

            dbTaskResult.IsDeleted = true;
            _context.Tasks.Update(dbTaskResult);
            var result = await _context.PushAsync();

            if (result.IsSuccess) await _notify.DeleteTaskNotificationAsync(dbTaskResult);
            return new ResultModel
            {
                IsSuccess = result.IsSuccess,
                Errors = result.Errors
            };
        }

        public async Task<ResultModel> DeletePermanentTaskAsync(Guid taskId)
        {
            if (taskId == Guid.Empty) return ExceptionMessagesEnum.NullParameter.ToErrorModel();

            var task = _context.Tasks.FirstOrDefault(x => x.Id == taskId);
            if (task == null) return ExceptionMessagesEnum.TaskNotFound.ToErrorModel();

            _context.Tasks.Remove(task);

            var result = await _context.PushAsync();

            return new ResultModel
            {
                IsSuccess = result.IsSuccess,
                Errors = result.Errors
            };
        }

        public async Task<ResultModel> RestoreTaskAsync(Guid taskId)
        {
            if (taskId == Guid.Empty) return ExceptionMessagesEnum.NullParameter.ToErrorModel();

            var dbTaskResult = _context.Tasks.FirstOrDefault(x => x.Id == taskId);
            if (dbTaskResult == null) return ExceptionMessagesEnum.TaskNotFound.ToErrorModel();

            dbTaskResult.IsDeleted = false;
            _context.Tasks.Update(dbTaskResult);
            var result = await _context.PushAsync();

            if (result.IsSuccess) await _notify.DeleteTaskNotificationAsync(dbTaskResult);
            return new ResultModel
            {
                IsSuccess = result.IsSuccess,
                Errors = result.Errors
            };
        }

        #endregion

        #region Task Items

        public async Task<ResultModel<Guid>> CreateTaskItemAsync(CreateTaskItemViewModel taskItem)
        {
            var dbTaskResult = _context.Tasks.FirstOrDefault(x => x.Id == taskItem.TaskId);
            if (dbTaskResult == null) return ExceptionMessagesEnum.TaskNotFound.ToErrorModel<Guid>();

            var taskModel = new TaskItem { Name = taskItem.Name, Task = dbTaskResult };

            _context.TaskItems.Add(taskModel);
            var result = await _context.PushAsync();

            return new ResultModel<Guid>
            {
                IsSuccess = result.IsSuccess,
                Result = taskModel.Id,
                Errors = result.Errors
            };
        }

        public async Task<ResultModel<Guid>> UpdateTaskItemAsync(UpdateTaskItemViewModel taskItem)
        {
            var dbTaskResult = _context.TaskItems.FirstOrDefault(x => x.Id == taskItem.Id);
            if (dbTaskResult == null) return ExceptionMessagesEnum.TaskNotFound.ToErrorModel<Guid>();

            dbTaskResult.Name = taskItem.Name;
            dbTaskResult.IsDone = taskItem.IsDone;
            _context.TaskItems.Update(dbTaskResult);

            var result = await _context.PushAsync();

            return new ResultModel<Guid>
            {
                IsSuccess = result.IsSuccess,
                Result = dbTaskResult.Id,
                Errors = result.Errors
            };
        }

        public async Task<ResultModel> DeleteTaskItemAsync(Guid taskItemId)
        {
            if (taskItemId == Guid.Empty) return ExceptionMessagesEnum.NullParameter.ToErrorModel();

            var task = _context.TaskItems.FirstOrDefault(x => x.Id == taskItemId);
            if (task == null) return ExceptionMessagesEnum.TaskNotFound.ToErrorModel();

            _context.TaskItems.Remove(task);

            var result = await _context.PushAsync();

            return new ResultModel
            {
                IsSuccess = result.IsSuccess,
                Errors = result.Errors
            };
        }

        private async Task<string> GenerateTaskNumberAsync()
        {
            const string number = "00001";
            var task = await _context.Tasks.LastOrDefaultAsync();
            if (task != null)
            {
                var lastNumber = task.TaskNumber.IsNumeric() ? int.Parse(task.TaskNumber) : 99900;
                return $"{++lastNumber:00000}";
            }

            return number;
        }

        #endregion
    }
}
