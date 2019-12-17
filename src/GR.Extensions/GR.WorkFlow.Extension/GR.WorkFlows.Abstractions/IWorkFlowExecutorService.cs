using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.WorkFlows.Abstractions.Models;
using GR.WorkFlows.Abstractions.ViewModels;

namespace GR.WorkFlows.Abstractions
{
    public interface IWorkFlowExecutorService
    {
        /// <summary>
        /// Register entity contract
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> RegisterEntityContractToWorkFlowAsync([Required] string entityName, Guid? workFlowId);

        /// <summary>
        /// Remove entity contract
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        Task<ResultModel> RemoveEntityContractToWorkFlowAsync([Required] string entityName, Guid? workFlowId);

        /// <summary>
        /// Check for registered contract to entity
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        Task<bool> IsAnyRegisteredContractToEntityAsync([Required] string entityName, Guid? workFlowId);

        /// <summary>
        /// Execute actions
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="transition"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task ExecuteActionsAsync(EntryState entry, Transition transition, Dictionary<string, string> data);

        /// <summary>
        /// Force execute transition actions
        /// </summary>
        /// <param name="entryId"></param>
        /// <param name="transitionId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<ResultModel> ForceExecuteTransitionActionsAsync(Guid? entryId, Guid? transitionId, Dictionary<string, string> data);

        /// <summary>
        /// Get roles for transition
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        Task<IEnumerable<GearRole>> GetAllowedRolesToTransitionAsync(Transition transition);

        /// <summary>
        /// Get next possible transitions
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        Task<IEnumerable<Transition>> GetNextTransitionsAsync(Transition transition);

        /// <summary>
        /// Get next transitions from state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Transition>>> GetNextTransitionsFromStateAsync([Required] State state);

        /// <summary>
        /// Get next states
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<State>>> GetNextStatesAsync([Required] State state);

        /// <summary>
        /// Get entry state
        /// </summary>
        /// <param name="entryId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<EntryState>>> GetEntryStatesAsync([Required] string entryId);


        /// <summary>
        /// Get entry state
        /// </summary>
        /// <param name="entryId"></param>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        Task<ResultModel<EntryState>> GetEntryStateAsync([Required] string entryId, [Required] Guid? workFlowId);

        /// <summary>
        /// Get next states for entry
        /// </summary>
        /// <param name="entryId"></param>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<StateGetViewModel>>> GetNextStatesForEntryAsync([Required] string entryId, [Required] Guid? workFlowId);

        /// <summary>
        /// Get entity contracts
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<WorkFlowEntityContract>>> GetEntityContractsAsync([Required] string entityName);

        /// <summary>
        /// Set start state for entry on all registered workflows
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entryId"></param>
        /// <returns></returns>
        Task<ResultModel> SetStartStateForEntryAsync([Required] string entityName, [Required] string entryId);

        /// <summary>
        /// Get next states from state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<State>>> GetNextStatesForAllowedRolesAsync([Required] State state);

        /// <summary>
        /// Change state for entry 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> ChangeStateForEntryAsync([Required]ObjectChangeStateViewModel model);

        /// <summary>
        /// Add entry changes to history
        /// </summary>
        /// <param name="state"></param>
        /// <param name="newState"></param>
        /// <returns></returns>
        Task<ResultModel> AddEntryChangesToHistoryAsync(EntryState state, Guid? newState);

        /// <summary>
        /// Get entry history
        /// </summary>
        /// <param name="workflowId"></param>
        /// <param name="entryId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<EntryHistoryViewModel>>> GetEntryHistoryByWorkflowIdAsync(Guid? workflowId, string entryId);

        /// <summary>
        /// Get workflow contracts
        /// </summary>
        /// <param name="workFLowId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<WorkFlowEntityContract>>> GetWorkflowContractsAsync(Guid? workFLowId);
    }
}