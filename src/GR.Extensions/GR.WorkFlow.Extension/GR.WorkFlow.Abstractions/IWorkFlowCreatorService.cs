﻿using System;
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

        /// <summary>
        /// Get all workflow
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<GetWorkFlowViewModel>>> GetAllWorkFlowsAsync();

        /// <summary>
        /// Enable or disable workflow
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        Task<ResultModel> EnableOrDisableWorkFlowAsync([Required] Guid? workFlowId, bool state);

        /// <summary>
        /// Update workflow data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateWorkFlowAsync([Required] UpdateWorkflowViewModel model);

        /// <summary>
        /// Update state additional settings
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateStateAdditionalSettingsAsync([Required] Guid? stateId, Dictionary<string, string> settings);

        /// <summary>
        /// Update workflow state
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateWorkflowStateAsync([Required] UpdateWorkFlowStateViewModel model);

        /// <summary>
        /// Get workflow states
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<StateGetViewModel>>> GetWorkFlowStatesAsync([Required] Guid? workFlowId);

        /// <summary>
        /// Get workflow for display
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        Task<ResultModel<WorkFlowGetViewModel>> GetWorkFlowByIdForDisplayAsync(Guid? workFlowId);
    }
}