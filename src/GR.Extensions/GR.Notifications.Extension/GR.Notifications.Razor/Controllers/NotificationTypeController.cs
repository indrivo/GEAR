using System.Threading.Tasks;
using GR.Core;
using GR.DynamicEntityStorage.Abstractions;
using GR.Notifications.Abstractions.Models.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.Notifications.Razor.Controllers
{
    [Authorize]
    public class NotificationTypeController : Controller
    {
        #region Inject
        /// <summary>
        /// Inject data dataService
        /// </summary>
        private readonly IDynamicService _service;

        public NotificationTypeController(IDynamicService service)
        {
            _service = service;
        }

        #endregion
        /// <summary>
        /// List of notification types
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get application roles list
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> NotificationTypesList(DTParameters param)
        {
            var (data, count) = await _service.Filter<NotificationTypes>(param.Search.Value, param.SortOrder, param.Start,
                param.Length);

            var finalResult = new DTResult<NotificationTypes>
            {
                Draw = param.Draw,
                Data = data,
                RecordsFiltered = count,
                RecordsTotal = data.Count
            };

            return Json(finalResult);
        }
    }
}