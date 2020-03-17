using Microsoft.AspNetCore.Mvc;
using GR.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using GR.Core.Helpers.Pagination;
using GR.Identity.Abstractions;
using GR.TaskManager.Abstractions;
using GR.TaskManager.Abstractions.Enums;
using GR.TaskManager.Abstractions.Helpers;
using GR.TaskManager.Abstractions.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace GR.TaskManager.Razor.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public sealed class TaskManagerController : Controller
    {

        #region Injectable

        /// <summary>
        /// Inject Task service
        /// </summary>
        private readonly ITaskManager _taskManager;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        #endregion

        public TaskManagerController(ITaskManager taskManager, IUserManager<GearUser> userManager)
        {
            _taskManager = taskManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Index page
        /// </summary>
        /// <returns></returns>
        [HttpGet("/TaskManager")]
        public IActionResult Index()
        {
            return View(new List<string>());
        }

        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<SelectList>))]
        public JsonResult GetTaskPriorityList()
        {
            var directions = from TaskPriority d in Enum.GetValues(typeof(TaskPriority))
                select new {ID = (int) d, Name = d.ToString()};
            return Json(new SelectList(directions, "ID", "Name", 0));
        }

        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<SelectList>))]
        public JsonResult GetTaskStatusList()
        {
            var directions = from Abstractions.Enums.TaskStatus d in Enum.GetValues(typeof(Abstractions.Enums.TaskStatus))
                select new {ID = (int) d, Name = d.ToString()};
            return Json(new SelectList(directions, "ID", "Name", 0));
        }

        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<SelectList>))]
        public JsonResult GetUsersList()
        {
            var users = _userManager.UserManager.Users.Where(x => x.TenantId == _userManager.CurrentUserTenantId).ToList();

            var directions = from GearUser d in users select new {ID = d.Id, Name = d.UserName};
            return Json(new SelectList(directions, "ID", "Name", 0));
        }

        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<GetTaskViewModel>))]
        public async Task<JsonResult> GetTask(Guid id)
        {
            if (id == Guid.Empty) return Json(ExceptionMessagesEnum.NullParameter.ToErrorModel());

            var response = await _taskManager.GetTaskAsync(id);
            return Json(response);
        }

        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<List<GetTaskViewModel>>))]
        public async Task<JsonResult> GetUserTasks(PageRequest request)
        {
            var userName = HttpContext.User.Identity.Name;

            var response = await _taskManager.GetUserTasksAsync(userName, request);
            return Json(response);
        }

        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<PagedResult<GetTaskViewModel>>))]
        public async Task<JsonResult> GetAssignedTasks(PageRequest request)
        {
            var user = await _userManager.GetCurrentUserAsync();

            if (user.Result == null) return Json(ExceptionMessagesEnum.UserNotFound.ToErrorModel());

            var response = await _taskManager.GetAssignedTasksAsync(user.Result.Id, user.Result.UserName, request);
            return Json(response);
        }


        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<PagedResult<GetTaskItemViewModel>>))]
        public async Task<JsonResult> GetTaskItems(Guid id)
        {
            if (id == Guid.Empty) return Json(ExceptionMessagesEnum.NullParameter.ToErrorModel());

            var response = await _taskManager.GetTaskItemsAsync(id);
            return Json(response);
        }

        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel<CreateTaskViewModel>))]
        public async Task<JsonResult> CreateTask(CreateTaskViewModel model)
        {
            if (!ModelState.IsValid) return Json(ModelState.ToErrorModel<CreateTaskViewModel>());

            var response = await _taskManager.CreateTaskAsync(model);
            return Json(response);
        }

        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel<UpdateTaskViewModel>))]
        public async Task<JsonResult> UpdateTask(UpdateTaskViewModel model)
        {
            if (!ModelState.IsValid) return Json(ModelState.ToErrorModel<UpdateTaskViewModel>());

            var response = await _taskManager.UpdateTaskAsync(model);
            return Json(response);
        }

        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> DeleteTask(Guid id)
        {
            if (id == Guid.Empty) return Json(ExceptionMessagesEnum.NullParameter.ToErrorModel());

            var response = await _taskManager.DeleteTaskAsync(id);
            return Json(response);
        }

        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> DeleteTaskPermanent(Guid id)
        {
            if (id == Guid.Empty) return Json(ExceptionMessagesEnum.NullParameter.ToErrorModel());

            var response = await _taskManager.DeletePermanentTaskAsync(id);
            return Json(response);
        }

        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> RestoreTask(Guid id)
        {
            if (id == Guid.Empty) return Json(ExceptionMessagesEnum.NullParameter.ToErrorModel());

            var response = await _taskManager.RestoreTaskAsync(id);
            return Json(response);
        }

        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel<CreateTaskItemViewModel>))]
        public async Task<JsonResult> CreateTaskItem(CreateTaskItemViewModel model)
        {
            if (!ModelState.IsValid) return Json(ModelState.ToErrorModel<CreateTaskItemViewModel>());

            var response = await _taskManager.CreateTaskItemAsync(model);
            return Json(response);
        }

        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel<UpdateTaskItemViewModel>))]
        public async Task<JsonResult> UpdateTaskItem(UpdateTaskItemViewModel model)
        {
            if (!ModelState.IsValid) return Json(ModelState.ToErrorModel<UpdateTaskItemViewModel>());

            var response = await _taskManager.UpdateTaskItemAsync(model);
            return Json(response);
        }

        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> DeleteTaskItem(Guid id)
        {
            if (id == Guid.Empty) return Json(ExceptionMessagesEnum.NullParameter.ToErrorModel());

            var response = await _taskManager.DeleteTaskItemAsync(id);
            return Json(response);
        }
    }
}
