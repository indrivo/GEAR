using Microsoft.AspNetCore.Mvc;

namespace GR.Core.Razor.Controllers
{
    public class HandlerController : Controller
    {
        /// <summary>
        /// Redirect to not found view
        /// </summary>
        /// <returns></returns>
        public new IActionResult NotFound()
        {
            return View();
        }
    }
}