using GR.Core.Razor.BaseControllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.Core.Razor.Controllers
{
    [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
    public class AdvancedSettingsController : BaseGearController
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}