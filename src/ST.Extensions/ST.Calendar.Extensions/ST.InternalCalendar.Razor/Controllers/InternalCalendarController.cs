using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ST.Calendar.Abstractions;
using ST.Calendar.Abstractions.Enums;
using ST.Calendar.Abstractions.Models;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Identity.Abstractions;

namespace ST.Calendar.Razor.Controllers
{
    [Authorize]
    public class InternalCalendarController : Controller
    {
        /// <summary>
        /// Inject Task service
        /// </summary>
        private readonly ICalendarManager _calendarManager;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<ApplicationUser> _userManager;

        public InternalCalendarController(ICalendarManager calendarManager, IUserManager<ApplicationUser> userManager)
        {
            _calendarManager = calendarManager;
            _userManager = userManager;
        }

        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<List<CalendarEvent>>))]
        public async Task<JsonResult> GetEvents(CalendarTimeLineType timeLine, DateTime dateTime)
        {
            var user = await _userManager.GetCurrentUserAsync();

            var response = await _calendarManager.GetEventsAsync(timeLine, dateTime,user.Result.Id.ToGuid());
            return Json(response);
        }

        /// <summary>
        /// Internal calendar
        /// </summary>
        /// <returns></returns>
        [HttpGet("calendar")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// External calendars
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ExternalCalendars()
        {
            return View();
        }
    }
}
