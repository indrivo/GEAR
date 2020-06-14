using System.Threading.Tasks;
using GR.Core.Razor.BaseControllers;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Modules.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace GR.Modules.Razor.Controllers
{
    [Admin]
    public class ModulesController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject module service
        /// </summary>
        private readonly IModuleService _moduleService;

        #endregion
        public ModulesController(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var modules = await _moduleService.GetAllModulesAsync();
            return View(modules.Result);
        }
    }
}
