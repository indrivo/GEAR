using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.BaseControllers;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.ModelBinders;
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
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    public class WorkFlowBuilderController : BaseGearController
    {
        #region Injectable
        /// <summary>
        /// Inject workflow service
        /// </summary>
        private readonly IWorkFlowCreatorService<WorkFlow> _workFlowCreatorService;

        /// <summary>
        /// Inject users manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;
        #endregion

        public WorkFlowBuilderController(IWorkFlowCreatorService<WorkFlow> workFlowCreatorService, IUserManager<GearUser> userManager)
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
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<Guid>))]
        public async Task<JsonResult> AddNewWorkflow([Required] AddNewWorkflowViewModel model)
        {
            return !ModelState.IsValid ? Json(new InvalidParametersResultModel<Guid>().AttachModelState(ModelState))
                : await JsonAsync(_workFlowCreatorService.AddNewWorkflowAsync(model));
        }

        /// <summary>
        /// Get workflow by id
        /// </summary>
        /// <param name="workflowId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<WorkFlowGetViewModel>))]
        public async Task<JsonResult> GetWorkFlowById([Required] Guid? workflowId)
            => await JsonAsync(_workFlowCreatorService.GetWorkFlowByIdForDisplayAsync(workflowId));

        /// <summary>
        /// Add new state to workflow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<Guid>))]
        public async Task<JsonResult> AddStateToWorkFlow([Required] AddNewStateViewModel model)
        {
            return !ModelState.IsValid ? Json(new InvalidParametersResultModel<Guid>().AttachModelState(ModelState))
                : await JsonAsync(_workFlowCreatorService.AddStateToWorkFlowAsync(model));
        }

        /// <summary>
        /// Create new transition between 2 states 
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<Guid>))]
        public async Task<JsonResult> CreateTransition([Required] Guid? workFlowId, [Required]AddTransitionViewModel model)
        {
            return !ModelState.IsValid ? Json(new InvalidParametersResultModel<Guid>().AttachModelState(ModelState))
                : await JsonAsync(_workFlowCreatorService.CreateTransitionAsync(workFlowId, model));
        }

        /// <summary>
        /// Get transition by id
        /// </summary>
        /// <param name="transitionId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<Transition>))]
        public async Task<JsonResult> GetTransitionById([Required]Guid? transitionId)
            => await JsonAsync(_workFlowCreatorService.GetTransitionByIdAsync(transitionId));

        /// <summary>
        /// Remove transition by id
        /// </summary>
        /// <param name="transitionId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> RemoveTransitionById([Required] Guid? transitionId)
            => await JsonAsync(_workFlowCreatorService.RemoveTransitionByIdAsync(transitionId));

        /// <summary>
        /// Set start state in workflow
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <param name="stateId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> SetStartStateInWorkflow([Required] Guid? workFlowId, [Required] Guid? stateId)
            => await JsonAsync(_workFlowCreatorService.SetStartStateInWorkflowAsync(workFlowId, stateId));

        /// <summary>
        /// Set end state in workflow
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <param name="stateId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> SetEndStateInWorkflow([Required] Guid? workFlowId, [Required] Guid? stateId)
            => await JsonAsync(_workFlowCreatorService.SetEndStateInWorkflowAsync(workFlowId, stateId));

        /// <summary>
        /// Add or update transition allowed user roles
        /// </summary>
        /// <param name="transitionId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> AddOrUpdateTransitionAllowedRoles([Required]Guid? transitionId, [Required]IEnumerable<Guid> roles)
         => await JsonAsync(_workFlowCreatorService.AddOrUpdateTransitionAllowedRolesAsync(transitionId, roles));

        /// <summary>
        /// Get allowed roles to participate in workflow
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<object>))]
        public async Task<JsonResult> GetRolesAllowedToParticipateInWorkflow()
        {
            var roles = await _userManager.RoleManager.Roles.ToListAsync();
            return Json(new SuccessResultModel<IEnumerable<object>>(roles.Select(x => new { x.Id, x.Name })));
        }

        /// <summary>
        /// Get all workflows
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<object>))]
        public async Task<JsonResult> GetAllWorkflows()
            => await JsonAsync(_workFlowCreatorService.GetAllWorkFlowsAsync());


        /// <summary>
        /// Enable or disable workflows
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> EnableOrDisableWorkFlow([Required] Guid? workFlowId, bool state)
            => await JsonAsync(_workFlowCreatorService.EnableOrDisableWorkFlowAsync(workFlowId, state));

        /// <summary>
        /// Update workflow data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> UpdateWorkFlowAsync([Required] UpdateWorkflowViewModel model)
         => ModelState.IsValid ? await JsonAsync(_workFlowCreatorService.UpdateWorkFlowAsync(model))
             : Json(new InvalidParametersResultModel<object>().AttachModelState(ModelState).ToBase());

        /// <summary>
        /// Update additional settings for workflow state
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> UpdateStateAdditionalSettings([Required] Guid? stateId, [ModelBinder(typeof(GearDictionaryBinder<string>))] Dictionary<string, string> settings)
        {
            return await JsonAsync(_workFlowCreatorService.UpdateStateAdditionalSettingsAsync(stateId, settings));
        }

        /// <summary>
        /// Update workflow state
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> UpdateWorkFlowStateAsync([Required] UpdateWorkFlowStateViewModel model)
            => ModelState.IsValid ? await JsonAsync(_workFlowCreatorService.UpdateWorkflowStateAsync(model))
                : Json(new InvalidParametersResultModel<object>().AttachModelState(ModelState).ToBase());

        /// <summary>
        /// Get workflow states
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<StateGetViewModel>>))]
        public async Task<JsonResult> GetWorkFlowStates([Required] Guid? workFlowId)
            => await JsonAsync(_workFlowCreatorService.GetWorkFlowStatesAsync(workFlowId));

        /// <summary>
        /// Get state by id
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<StateGetViewModel>))]
        public async Task<JsonResult> GetStateById([Required] Guid? stateId)
            => await JsonAsync(_workFlowCreatorService.GetStateByIdAsync(stateId));

        /// <summary>
        /// Remove state 
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> RemoveState([Required] Guid? stateId)
            => await JsonAsync(_workFlowCreatorService.RemoveStateAsync(stateId));

        /// <summary>
        /// Remove workflow 
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> RemoveWorkflow([Required] Guid? workFlowId)
            => await JsonAsync(_workFlowCreatorService.RemoveWorkFlowAsync(workFlowId));

        /// <summary>
        /// Add or update transition actions
        /// </summary>
        /// <param name="transitionId"></param>
        /// <param name="actionHandlers"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> AddOrUpdateTransitionActions([Required] Guid? transitionId,
            IEnumerable<Guid> actionHandlers)
            => await JsonAsync(_workFlowCreatorService.AddOrUpdateTransitionActionsAsync(transitionId, actionHandlers));

        /// <summary>
        /// Get all registered actions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<WorkflowAction>>))]
        public async Task<JsonResult> GetAllRegisteredActions()
            => await JsonAsync(_workFlowCreatorService.GetAllRegisteredActionsAsync());


        /// <summary>
        /// Update transition name
        /// </summary>
        /// <param name="transitionId"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> UpdateTransitionName(Guid? transitionId, string newName)
            => await JsonAsync(_workFlowCreatorService.UpdateTransitionNameAsync(transitionId, newName));

        /// <summary>
        /// Update state general info
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="newName"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> UpdateStateGeneralInfo([Required]Guid? stateId, [Required]string newName, string description)
            => await JsonAsync(_workFlowCreatorService.UpdateStateGeneralInfoAsync(stateId, newName, description));
    }
}