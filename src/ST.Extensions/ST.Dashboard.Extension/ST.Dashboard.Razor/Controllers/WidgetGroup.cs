using Microsoft.AspNetCore.Mvc;

namespace ST.Dashboard.Razor.Controllers
{
    public class WidgetGroup : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}