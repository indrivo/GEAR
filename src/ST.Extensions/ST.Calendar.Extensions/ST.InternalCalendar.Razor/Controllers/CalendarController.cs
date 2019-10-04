using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ST.Calendar.Abstractions;
using ST.Identity.Abstractions;

namespace ST.Calendar.Razor.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        /// <summary>
        /// Inject Task service
        /// </summary>
        private readonly ICalendarManager _calendarManager;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<ApplicationUser> _userManager;

        public CalendarController(ICalendarManager calendarManager, IUserManager<ApplicationUser> userManager)
        {
            _calendarManager = calendarManager;
            _userManager = userManager;
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
