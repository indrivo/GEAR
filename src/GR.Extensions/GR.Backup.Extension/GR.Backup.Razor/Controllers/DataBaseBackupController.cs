using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GR.Core;

namespace GR.Backup.Razor.Controllers
{
    [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
    public class DataBaseBackupController : Controller
    {
        /// <summary>
        /// Backup manager
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}