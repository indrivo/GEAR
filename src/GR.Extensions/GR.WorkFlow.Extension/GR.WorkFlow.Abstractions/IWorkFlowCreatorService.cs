using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.WorkFlows.Abstractions.Models;
using GR.WorkFlows.Abstractions.ViewModels;

namespace GR.WorkFlows.Abstractions
{
    public interface IWorkFlowCreatorService<TWorkFlow> where TWorkFlow : WorkFlow
    {
        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        Task<ResultModel<TWorkFlow>> GetWorkFlowByIdAsync(Guid? workFlowId);

        /// <summary>
        /// Add new workflow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> AddNewWorkflowAsync(AddNewWorkflowViewModel model);

        /// <summary>
        /// Add new state
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<Guid?>> AddStateToWorkFlowAsync(AddNewStateViewModel model);

        /// <summary>
        /// Create new transition
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> CreateTransitionAsync([Required] Guid? workFlowId, AddTransitionViewModel model);

        /// <summary>
        /// Get transition by id
        /// </summary>
        /// <param name="transitionId"></param>
        /// <returns></returns>
        Task<ResultModel<Transition>> GetTransitionByIdAsync(Guid? transitionId);

        /// <summary>
        /// Remove transition
        /// </summary>
        /// <param name="transitionId"></param>
        /// <returns></returns>
        Task<ResultModel> RemoveTransitionByIdAsync(Guid? transitionId);

        /// <summary>
        /// Set start state
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <param name="stateId"></param>
        /// <returns></returns>
        Task<ResultModel> SetStartStateInWorkflowAsync(Guid? workFlowId, Guid? stateId);

        /// <summary>
        /// Set end state
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <param name="stateId"></param>
        /// <returns></returns>
        Task<ResultModel> SetEndStateInWorkflowAsync(Guid? workFlowId, Guid? stateId);

        /// <summary>
        /// Add or update allowed roles to transition
        /// </summary>
        /// <param name="transitionId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        Task<ResultModel> AddOrUpdateTransitionAllowedRolesAsync(Guid? transitionId, IEnumerable<Guid> roles);
    }
}