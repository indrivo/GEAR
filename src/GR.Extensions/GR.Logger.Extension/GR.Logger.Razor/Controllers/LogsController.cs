using Microsoft.AspNetCore.Mvc;

namespace GR.Logger.Razor.Controllers
{
    public class LogsController : Controller
    {
        /// <summary>
        /// Logs
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}