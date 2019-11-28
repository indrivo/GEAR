using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Identity.Abstractions;
using GR.WorkFlows.Abstractions;
using GR.WorkFlows.Abstractions.Models;
using GR.WorkFlows.Abstractions.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GR.WorkFlows.Razor.Controllers
{
    [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
    public class WorkFlowBuilderController : Controller
    {
        #region Injectable
        /// <summary>
        /// Inject workflow service
        /// </summary>
        private readonly IWorkFlowCreatorService<WorkFlow> _workFlowCreatorService;

        /// <summary>
        /// Inject users manager
        /// </summary>
        private readonly IUserManager<ApplicationUser> _userManager;
        #endregion

        public WorkFlowBuilderController(IWorkFlowCreatorService<WorkFlow> workFlowCreatorService, IUserManager<ApplicationUser> userManager)
        {
            _workFlowCreatorService = workFlowCreatorService;
            _userManager = userManager;
        }

        /// <summary>
        /// Show list of workflow
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Add new workflow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel<Guid>))]
        public async Task<JsonResult> AddNewWorkflow([Required] AddNewWorkflowViewModel model)
        {
            return !ModelState.IsValid ? Json(new InvalidParametersResultModel<Guid>().AttachModelState(ModelState))
                : Json(await _workFlowCreatorService.AddNewWorkflowAsync(model));
        }

        /// <summary>
        /// Get workflow by id
        /// </summary>
        /// <param name="workflowId"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<WorkFlow>))]
        public async Task<JsonResult> GetWorkFlowById([Required] Guid? workflowId)
            => Json(await _workFlowCreatorService.GetWorkFlowByIdAsync(workflowId));

        /// <summary>
        /// Add new state to workflow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel<Guid>))]
        public async Task<JsonResult> AddStateToWorkFlow([Required] AddNewStateViewModel model)
        {
            return !ModelState.IsValid ? Json(new InvalidParametersResultModel<Guid>().AttachModelState(ModelState))
                : Json(await _workFlowCreatorService.AddStateToWorkFlowAsync(model));
        }

        /// <summary>
        /// Create new transition between 2 states 
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel<Guid>))]
        public async Task<JsonResult> CreateTransition([Required] Guid? workFlowId, [Required]AddTransitionViewModel model)
        {
            return !ModelState.IsValid ? Json(new InvalidParametersResultModel<Guid>().AttachModelState(ModelState))
                : Json(await _workFlowCreatorService.CreateTransitionAsync(workFlowId, model));
        }

        /// <summary>
        /// Get transition by id
        /// </summary>
        /// <param name="transitionId"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<Transition>))]
        public async Task<JsonResult> GetTransitionById([Required]Guid? transitionId)
            => Json(await _workFlowCreatorService.GetTransitionByIdAsync(transitionId));

        /// <summary>
        /// Remove transition by id
        /// </summary>
        /// <param name="transitionId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> RemoveTransitionById([Required] Guid? transitionId)
            => Json(await _workFlowCreatorService.RemoveTransitionByIdAsync(transitionId));

        /// <summary>
        /// Set start state in workflow
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <param name="stateId"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> SetStartStateInWorkflow([Required] Guid? workFlowId, [Required] Guid? stateId)
            => Json(await _workFlowCreatorService.SetStartStateInWorkflowAsync(workFlowId, stateId));

        /// <summary>
        /// Set end state in workflow
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <param name="stateId"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> SetEndStateInWorkflow([Required] Guid? workFlowId, [Required] Guid? stateId)
            => Json(await _workFlowCreatorService.SetEndStateInWorkflowAsync(workFlowId, stateId));

        /// <summary>
        /// Add or update transition allowed user roles
        /// </summary>
        /// <param name="transitionId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> AddOrUpdateTransitionAllowedRoles([Required]Guid? transitionId, [Required]IEnumerable<Guid> roles)
         => Json(await _workFlowCreatorService.AddOrUpdateTransitionAllowedRolesAsync(transitionId, roles));

        /// <summary>
        /// Get allowed roles to participate in workflow
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<object>))]
        public async Task<JsonResult> GetRolesAllowedToParticipateInWorkflow()
        {
            var roles = await _userManager.RoleManager.Roles.ToListAsync();
            return Json(roles.Select(x => new { x.Id, x.Name }));
        }
    }
}