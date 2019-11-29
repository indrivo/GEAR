using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Identity.Abstractions;
using GR.WorkFlows.Abstractions;
using GR.WorkFlows.Abstractions.Helpers.ActionHandlers;
using GR.WorkFlows.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.WorkFlows
{
    public class WorkFlowExecutor : IWorkFlowExecutor
    {
        #region Injectable
        /// <summary>
        /// Inject workflow service
        /// </summary>

        private readonly IWorkFlowCreatorService<WorkFlow> _workFlowCreatorService;

        /// <summary>
        /// Inject workflow context
        /// </summary>
        private readonly IWorkFlowContext _workFlowContext;

        #endregion

        public WorkFlowExecutor(IWorkFlowCreatorService<WorkFlow> workFlowCreatorService, IWorkFlowContext workFlowContext)
        {
            _workFlowCreatorService = workFlowCreatorService;
            _workFlowContext = workFlowContext;
        }

        /// <summary>
        /// Register entity contract
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        public async Task<ResultModel<Guid>> RegisterEntityContractToWorkFlowAsync([Required]string entityName, Guid? workFlowId)
        {
            if (entityName.IsNullOrEmpty() || workFlowId == null) return new InvalidParametersResultModel<Guid>();
            var workFlowRequest = await _workFlowCreatorService.GetWorkFlowByIdAsync(workFlowId);
            if (!workFlowRequest.IsSuccess) return workFlowRequest.Map<Guid>();
            if (await IsAnyRegisteredContractToEntityAsync(entityName, workFlowId))
                return new InvalidParametersResultModel<Guid>("This workflow for the entity has already been recorded");
            var contract = new WorkFlowEntityContract
            {
                WorkFlowId = workFlowId.GetValueOrDefault(),
                EntityName = entityName
            };
            await _workFlowContext.Contracts.AddAsync(contract);
            var dbRequest = await _workFlowContext.PushAsync();
            return dbRequest.Map(contract.Id);
        }

        /// <summary>
        /// Check for registered contract to entity
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        public async Task<bool> IsAnyRegisteredContractToEntityAsync([Required]string entityName, Guid? workFlowId)
            => await _workFlowContext.Contracts.AnyAsync(x => x.WorkFlowId.Equals(workFlowId) && x.EntityName.Equals(entityName));

        /// <summary>
        /// Force execute transition actions
        /// </summary>
        /// <param name="entryId"></param>
        /// <param name="transitionId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<ResultModel> ForceExecuteTransitionActionsAsync(Guid? entryId, Guid? transitionId, Dictionary<string, object> data)
        {
            var entryState = await _workFlowContext.EntryStates
                .Include(x => x.Contract)
                .ThenInclude(x => x.WorkFlow)
                .FirstOrDefaultAsync(x => x.Id.Equals(entryId));
            if (entryState == null) return new InvalidParametersResultModel();
            var workFlowRequest = await _workFlowCreatorService.GetWorkFlowByIdAsync(entryState.Contract.WorkFlowId);
            if (!workFlowRequest.IsSuccess) return workFlowRequest.ToBase();
            var workFlow = workFlowRequest.Result;
            var transition = workFlow.Transitions.FirstOrDefault(x => x.Id.Equals(transitionId));
            await ExecuteActionsAsync(transition, data);
            return new SuccessResultModel<object>().ToBase();
        }

        /// <summary>
        /// Get roles for transition
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<ApplicationRole>> GetAllowedRolesToTransitionAsync(Transition transition)
        {
            Arg.NotNull(transition, nameof(GetAllowedRolesToTransitionAsync));
            var roleIds = transition?.TransitionRoles?.Select(x => x.RoleId).ToList() ?? new List<Guid>();
            var roles = await IoC.Resolve<IUserManager<ApplicationUser>>().FindRolesByIdAsync(roleIds);
            return roles.ToList();
        }

        /// <summary>
        /// Get next transitions
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Transition>> GetNextTransitionsAsync(Transition transition)
        {
            Arg.NotNull(transition, nameof(GetNextTransitionsAsync));
            var nextState = transition.ToState;
            if (nextState == null) return new List<Transition>();
            var nextTransitions = await _workFlowContext.Transitions
                .Include(x => x.FromState)
                .Include(x => x.ToState)
                .Include(x => x.WorkFlow)
                .Include(x => x.TransitionRoles)
                .Include(x => x.TransitionActions)
                .Where(x => x.FromStateId.Equals(nextState.Id)).ToListAsync();

            return nextTransitions;
        }

        /// <summary>
        /// Execute actions
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task ExecuteActionsAsync(Transition transition, Dictionary<string, object> data)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var actions = transition.TransitionActions.Select(x => x.Action).ToList();
            var nextTransitions = await GetNextTransitionsAsync(transition);
            foreach (var action in actions)
            {
                try
                {
                    var type = assembly.GetType(action.ClassNameWithNameSpace);
                    var activatedObject = (BaseWorkFlowAction)Activator.CreateInstance(type, transition, nextTransitions);
                    await activatedObject.InvokeExecuteAsync(data);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
