using Microsoft.AspNetCore.Mvc;
using ST.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using ST.Identity.Abstractions;
using ST.TaskManager.Abstractions;
using ST.TaskManager.Abstractions.Enums;
using ST.TaskManager.Abstractions.Helpers;
using ST.TaskManager.Abstractions.Models.ViewModels;
using TaskStatus = System.Threading.Tasks.TaskStatus;


namespace ST.TaskManager.Razor.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public sealed class TaskManagerController : Controller
    {
        /// <summary>
        /// Inject Task service
        /// </summary>
        private readonly ITaskManager _taskManager;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<ApplicationUser> _userManager;

        public TaskManagerController(ITaskManager taskManager, IUserManager<ApplicationUser> userManager)
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
            return View();
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
            var directions = from TaskStatus d in Enum.GetValues(typeof(TaskStatus))
                select new { ID = (int) d, Name = d.ToString()};
            return Json(new SelectList(directions, "ID", "Name", 0));
        }

        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<SelectList>))]
        public JsonResult GetUsersList()
        {
            var users = _userManager.UserManager.Users.Where(x => x.TenantId == _userManager.CurrentUserTenantId).ToList();

            var directions = from ApplicationUser d in users select new {ID = d.Id, Name = d.UserName};
            return Json(new SelectList(directions, "ID", "Name", 0));
        }

        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<GetTaskViewModel>))]
        public async Task<JsonResult> GetTask(Guid id)
        {
            if (id == Guid.Empty) return Json(ExceptionHandler.ReturnErrorModel(ExceptionMessagesEnum.NullParameter));

            var response = await _taskManager.GetTaskAsync(id);
            return Json(response);
        }

        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<List<GetTaskViewModel>>))]
        public async Task<JsonResult> GetUserTasks()
        {
            var userName = HttpContext.User.Identity.Name;

            var response = await _taskManager.GetUserTasksAsync(userName);
            return Json(response);
        }

        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<List<GetTaskViewModel>>))]
        public async Task<JsonResult> GetAssignedTasks()
        {
            var userId = _userManager.CurrentUserTenantId;

            var response = await _taskManager.GetAssignedTasksAsync(userId ?? Guid.Empty);
            return Json(response);
        }


        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<List<TaskItemViewModel>>))]
        public async Task<JsonResult> GetTaskItems(Guid id)
        {
            if (id == Guid.Empty) return Json(ExceptionHandler.ReturnErrorModel(ExceptionMessagesEnum.NullParameter));

            var response = await _taskManager.GetTaskItemsAsync(id);
            return Json(response);
        }

        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel<CreateTaskViewModel>))]
        public async Task<JsonResult> CreateTask(CreateTaskViewModel model)
        {
            if (!ModelState.IsValid) return Json(ExceptionHandler.ReturnErrorModel<CreateTaskViewModel>(ModelState));

            var response = await _taskManager.CreateTaskAsync(model);
            return Json(response);
        }

        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel<UpdateTaskViewModel>))]
        public async Task<JsonResult> UpdateTask(UpdateTaskViewModel model)
        {
            if (!ModelState.IsValid) return Json(ExceptionHandler.ReturnErrorModel<CreateTaskViewModel>(ModelState));

            var response = await _taskManager.UpdateTaskAsync(model);
            return Json(response);
        }

        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> DeleteTask(Guid id)
        {
            if (id == Guid.Empty) return Json(ExceptionHandler.ReturnErrorModel(ExceptionMessagesEnum.NullParameter));

            var response = await _taskManager.DeleteTaskAsync(id);
            return Json(response);
        }

        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> DeleteTaskPermanent(Guid id)
        {
            if (id == Guid.Empty) return Json(ExceptionHandler.ReturnErrorModel(ExceptionMessagesEnum.NullParameter));

            var response = await _taskManager.DeletePermanentTaskAsync(id);
            return Json(response);
        }

        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel<TaskItemViewModel>))]
        public async Task<JsonResult> CreateTaskItem(TaskItemViewModel model)
        {
            if (!ModelState.IsValid) return Json(ExceptionHandler.ReturnErrorModel<CreateTaskViewModel>(ModelState));

            var response = await _taskManager.CreateTaskItemAsync(model);
            return Json(response);
        }

        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel<TaskItemViewModel>))]
        public async Task<JsonResult> UpdateTaskItem(TaskItemViewModel model)
        {
            if (!ModelState.IsValid) return Json(ExceptionHandler.ReturnErrorModel<CreateTaskViewModel>(ModelState));

            var response = await _taskManager.UpdateTaskItemAsync(model);
            return Json(response);
        }

        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> DeleteTaskItem(Guid id)
        {
            if (id == Guid.Empty) return Json(ExceptionHandler.ReturnErrorModel(ExceptionMessagesEnum.NullParameter));

            var response = await _taskManager.DeleteTaskItemAsync(id);
            return Json(response);
        }
    }
}
