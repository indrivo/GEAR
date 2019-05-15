using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ST.InternalCalendar.Razor.Controllers
{
    [Authorize]
    public class InternalCalendarController : Controller
    {
        public InternalCalendarController()
        {

        }

        /// <summary>
        /// Internal calendar
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("calendar")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// External calendars
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("external-calendars")]
        public IActionResult ExternalCalendars()
        {
            return View();
        }
    }
}
