using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.WorkFlows.Abstractions.Models;

namespace GR.WorkFlows.Abstractions
{
    public interface IWorkFlowExecutor
    {
        /// <summary>
        /// Register entity contract
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> RegisterEntityContractToWorkFlowAsync([Required] string entityName, Guid? workFlowId);

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
        /// <param name="transition"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task ExecuteActionsAsync(Transition transition, Dictionary<string, object> data);

        /// <summary>
        /// Force execute transition actions
        /// </summary>
        /// <param name="entryId"></param>
        /// <param name="transitionId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<ResultModel> ForceExecuteTransitionActionsAsync(Guid? entryId, Guid? transitionId, Dictionary<string, object> data);

        /// <summary>
        /// Get roles for transition
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        Task<IEnumerable<ApplicationRole>> GetAllowedRolesToTransitionAsync(Transition transition);

        /// <summary>
        /// Get next possible transitions
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        Task<IEnumerable<Transition>> GetNextTransitionsAsync(Transition transition);
    }
}