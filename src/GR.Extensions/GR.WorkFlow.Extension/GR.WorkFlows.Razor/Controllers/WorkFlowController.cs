using GR.WorkFlows.Abstractions;
using GR.WorkFlows.Abstractions.Models;
using Microsoft.AspNetCore.Mvc;

namespace GR.WorkFlows.Razor.Controllers
{
    public class WorkFlowController : Controller
    {
        #region Injectable
        /// <summary>
        /// Inject workflow service
        /// </summary>
        private readonly IWorkFlowCreatorService<WorkFlow> _workFlowCreatorService;
        #endregion

        public WorkFlowController(IWorkFlowCreatorService<WorkFlow> workFlowCreatorService)
        {
            _workFlowCreatorService = workFlowCreatorService;
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}