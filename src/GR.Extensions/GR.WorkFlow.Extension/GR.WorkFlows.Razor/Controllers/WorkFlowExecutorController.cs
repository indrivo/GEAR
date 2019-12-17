using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using GR.Core.Attributes.Documentation;
using GR.Core.BaseControllers;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.WorkFlows.Abstractions;
using GR.WorkFlows.Abstractions.Models;
using GR.WorkFlows.Abstractions.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.WorkFlows.Razor.Controllers
{
    [Authorize]
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    [Documentation("Api that provide methods for workflows bind to an entity")]
    public sealed class WorkFlowExecutorController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject workflow executor
        /// </summary>
        private readonly IWorkFlowExecutorService _workFlowExecutorService;
        #endregion

        public WorkFlowExecutorController(IWorkFlowExecutorService workFlowExecutorService)
        {
            _workFlowExecutorService = workFlowExecutorService;
        }

        /// <summary>
        /// Register entity contract
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<Guid>))]
        public async Task<JsonResult> RegisterEntityContractToWorkFlow([Required] string entityName, Guid? workFlowId)
            => await JsonAsync(_workFlowExecutorService.RegisterEntityContractToWorkFlowAsync(entityName, workFlowId));


        /// <summary>
        /// Check if any registered contracts
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(bool))]
        public async Task<JsonResult> IsAnyRegisteredContractToEntity([Required] string entityName, Guid? workFlowId)
            => await JsonAsync(_workFlowExecutorService.IsAnyRegisteredContractToEntityAsync(entityName, workFlowId));


        /// <summary>
        /// Get entry states for all workflows
        /// </summary>
        /// <param name="entryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<EntryState>>))]
        public async Task<JsonResult> GetEntryStates([Required] string entryId)
            => await JsonAsync(_workFlowExecutorService.GetEntryStatesAsync(entryId));

        /// <summary>
        /// Get entry state by workflow id
        /// </summary>
        /// <param name="entryId"></param>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<EntryState>))]
        public async Task<JsonResult> GetEntryState([Required] string entryId, [Required] Guid? workFlowId)
            => await JsonAsync(_workFlowExecutorService.GetEntryStateAsync(entryId, workFlowId));


        /// <summary>
        /// Get next states for entry
        /// </summary>
        /// <param name="entryId"></param>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<StateGetViewModel>>))]
        public async Task<JsonResult> GetNextStatesForEntry([Required] string entryId, [Required] Guid? workFlowId)
            => await JsonAsync(_workFlowExecutorService.GetNextStatesForEntryAsync(entryId, workFlowId));


        /// <summary>
        /// Get entity contracts
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<WorkFlowEntityContract>>))]
        public async Task<JsonResult> GetEntityContracts([Required] string entityName)
            => await JsonAsync(_workFlowExecutorService.GetEntityContractsAsync(entityName));


        /// <summary>
        /// Init workflow attached to entity entry
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entryId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> SetStartStateForEntry([Required] string entityName, [Required] string entryId)
            => await JsonAsync(_workFlowExecutorService.SetStartStateForEntryAsync(entityName, entryId));


        /// <summary>
        /// Change state for entry
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> ChangeStateForEntry([Required] ObjectChangeStateViewModel model)
        {
            if (!ModelState.IsValid) return Json(new InvalidParametersResultModel().AttachModelState(ModelState));
            return await JsonAsync(_workFlowExecutorService.ChangeStateForEntryAsync(model));
        }


        /// <summary>
        /// Remove entity contract
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> RemoveEntityContractToWorkFlow([Required] string entityName, Guid? workFlowId)
            => await JsonAsync(_workFlowExecutorService.RemoveEntityContractToWorkFlowAsync(entityName, workFlowId));

        /// <summary>
        /// Get entry history
        /// </summary>
        /// <param name="workflowId"></param>
        /// <param name="entryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<EntryHistoryViewModel>>))]
        public async Task<JsonResult> GetEntryHistoryByWorkflowId([Required]Guid? workflowId, [Required]string entryId)
            => await JsonAsync(_workFlowExecutorService.GetEntryHistoryByWorkflowIdAsync(workflowId, entryId));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workFLowId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<WorkFlowEntityContract>>))]
        public async Task<JsonResult> GetWorkflowContracts(Guid? workFLowId)
            => await JsonAsync(_workFlowExecutorService.GetWorkflowContractsAsync(workFLowId));
    }
}