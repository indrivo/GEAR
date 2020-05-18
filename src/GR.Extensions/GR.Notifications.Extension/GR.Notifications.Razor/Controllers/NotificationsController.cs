using GR.Core.Attributes.Documentation;
using GR.Core.Helpers.Global;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.Notifications.Razor.Controllers
{
    [Author(Authors.LUPEI_NICOLAE)]
    [Authorize]
    public class NotificationsController : Controller
    {
        /// <summary>
        /// Manage user notifications
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index() => View();
    }
}
