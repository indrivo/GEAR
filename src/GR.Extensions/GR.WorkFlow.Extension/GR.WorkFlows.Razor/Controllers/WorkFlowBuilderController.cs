using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers.Global;
using GR.Core.Razor.BaseControllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.WorkFlows.Razor.Controllers
{
    [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    public class WorkFlowBuilderController : BaseGearController
    {
        /// <summary>
        /// Show list of workflow
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}