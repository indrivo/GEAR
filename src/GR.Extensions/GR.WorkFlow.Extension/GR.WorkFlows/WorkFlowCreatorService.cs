using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.Identity.Abstractions;
using GR.WorkFlows.Abstractions;
using GR.WorkFlows.Abstractions.Helpers;
using GR.WorkFlows.Abstractions.Helpers.Errors;
using GR.WorkFlows.Abstractions.Models;
using GR.WorkFlows.Abstractions.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GR.WorkFlows
{
    [Author(Authors.LUPEI_NICOLAE, 1.1, "Add base implementation")]
    [Documentation("Service for create workflow")]
    public class WorkFlowCreatorService : IWorkFlowCreatorService<WorkFlow>
    {
        #region Injectable

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly IWorkFlowContext _workFlowContext;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        #endregion

        public WorkFlowCreatorService(IWorkFlowContext workFlowContext, IUserManager<GearUser> userManager)
        {
            _workFlowContext = workFlowContext;
            _userManager = userManager;
        }

        /// <summary>
        /// Get flow by id
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<WorkFlow>> GetWorkFlowByIdAsync(Guid? workFlowId)
        {
            if (workFlowId == null) return new InvalidParametersResultModel<WorkFlow>();
            var workFlow = await _workFlowContext.WorkFlows
                .AsNoTracking()
                .Include(x => x.States)
                .Include(x => x.Transitions)
                .ThenInclude(x => x.TransitionActions)
                .ThenInclude(x => x.Action)
                .Include(x => x.Transitions)
                .ThenInclude(x => x.FromState)
                .Include(x => x.Transitions)
                .ThenInclude(x => x.ToState)
                .Include(x => x.Transitions)
                .ThenInclude(x => x.TransitionRoles)
                .FirstOrDefaultAsync(x => x.Id.Equals(workFlowId));
            if (workFlow == null) return new NotFoundResultModel<WorkFlow>();
            return new SuccessResultModel<WorkFlow>(workFlow);
        }

        /// <summary>
        /// Get workflow by id for display
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<WorkFlowGetViewModel>> GetWorkFlowByIdForDisplayAsync(Guid? workFlowId)
        {
            var workFlowRequest = await GetWorkFlowByIdAsync(workFlowId);
            return !workFlowRequest.IsSuccess ? workFlowRequest.Map<WorkFlowGetViewModel>()
                : new SuccessResultModel<WorkFlowGetViewModel>(WorkFlowMapper.Map(workFlowRequest.Result));
        }

        /// <summary>
        /// Add new workflow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> AddNewWorkflowAsync(AddNewWorkflowViewModel model)
        {
            Arg.NotNull(model, nameof(AddNewWorkflowViewModel));
            var workflow = WorkFlowMapper.Map(model);
            await _workFlowContext.WorkFlows.AddAsync(workflow);
            var dbRequest = await _workFlowContext.PushAsync();
            return dbRequest.Map(workflow.Id);
        }

        /// <summary>
        /// Update workflow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> UpdateWorkFlowAsync([Required] UpdateWorkflowViewModel model)
        {
            if (model == null) return new InvalidParametersResultModel<object>().ToBase();
            var workFlowRequest = await GetWorkFlowWhitOutIncludesAsync(model.Id);
            if (!workFlowRequest.IsSuccess) return workFlowRequest.ToBase();
            var workFlow = workFlowRequest.Result;
            workFlow.Name = model.Name;
            workFlow.Description = model.Description;
            workFlow.Enabled = model.Enabled;
            _workFlowContext.WorkFlows.Update(workFlow);
            return await _workFlowContext.PushAsync();
        }

        /// <summary>
        /// Enable or disable workflow
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> EnableOrDisableWorkFlowAsync([Required] Guid? workFlowId, bool state)
        {
            var workFlowRequest = await GetWorkFlowWhitOutIncludesAsync(workFlowId);
            if (!workFlowRequest.IsSuccess) return workFlowRequest.ToBase();
            var workFlow = workFlowRequest.Result;
            workFlow.Enabled = state;
            _workFlowContext.WorkFlows.Update(workFlow);
            return await _workFlowContext.PushAsync();
        }

        /// <summary>
        /// Add new state
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid?>> AddStateToWorkFlowAsync(AddNewStateViewModel model)
        {
            Arg.NotNull(model, nameof(AddNewStateViewModel));
            var response = new ResultModel<Guid?>();
            var workflowRequest = await GetWorkFlowByIdAsync(model.WorkFlowId);
            if (!workflowRequest.IsSuccess) return workflowRequest.Map<Guid?>();
            var workFlow = workflowRequest.Result;
            var checkDuplicateStateName = workFlow.States.FirstOrDefault(x => x.Name.Equals(model.Name));
            if (checkDuplicateStateName != null)
            {
                response.Errors.Add(new ErrorModel(nameof(WorkFlowErrorCodes.GRWF_0x104), WorkFlowErrorCodes.GRWF_0x104));
                return response;
            }

            var state = WorkFlowMapper.Map(model);

            if (!workFlow.States.Any()) state.IsStartState = true;

            await _workFlowContext.States.AddAsync(state);
            var dbRequest = await _workFlowContext.PushAsync();

            return dbRequest.Map<Guid?>(state.Id);
        }

        /// <summary>
        /// Update workflow state
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> UpdateWorkflowStateAsync([Required]UpdateWorkFlowStateViewModel model)
        {
            if (model == null) return new InvalidParametersResultModel();
            var state = await _workFlowContext.States
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(model.StateId));
            if (state == null) return new NotFoundResultModel();
            state.AdditionalSettings = model.SerializeAsJson();
            state.Name = model.Name;
            state.Description = model.Description;
            _workFlowContext.States.Update(state);
            return await _workFlowContext.PushAsync();
        }

        /// <summary>
        /// Update state settings
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> UpdateStateAdditionalSettingsAsync([Required]Guid? stateId, Dictionary<string, string> settings)
        {
            if (stateId == null) return new InvalidParametersResultModel();
            var state = await _workFlowContext.States
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(stateId));
            if (state == null) return new NotFoundResultModel();
            state.AdditionalSettings = settings.SerializeAsJson();
            _workFlowContext.States.Update(state);
            return await _workFlowContext.PushAsync();
        }


        /// <summary>
        /// Set start state
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public async Task<ResultModel> SetStartStateInWorkflowAsync(Guid? workFlowId, Guid? stateId)
        {
            var workflowRequest = await GetWorkFlowByIdAsync(workFlowId);
            if (!workflowRequest.IsSuccess) return workflowRequest.ToBase();
            var workFlow = workflowRequest.Result;

            var newStartState = workFlow.States.FirstOrDefault(x => x.Id.Equals(stateId));
            if (newStartState == null) return new NotFoundResultModel<object>().ToBase();
            if (newStartState.IsStartState) return new SuccessResultModel<object>().ToBase();
            var response = new ResultModel();
            if (newStartState.IsEndState)
            {
                response.Errors.Add(new ErrorModel(nameof(WorkFlowErrorCodes.GRWF_0x105), WorkFlowErrorCodes.GRWF_0x105));
                return response;
            }

            var toUpdate = workFlow.States.Select(x =>
            {
                x.IsStartState = x.Id.Equals(stateId);
                return x;
            });

            _workFlowContext.States.UpdateRange(toUpdate);

            return await _workFlowContext.PushAsync();
        }

        /// <summary>
        /// Set end state
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public async Task<ResultModel> SetEndStateInWorkflowAsync(Guid? workFlowId, Guid? stateId)
        {
            var workflowRequest = await GetWorkFlowByIdAsync(workFlowId);
            if (!workflowRequest.IsSuccess) return workflowRequest.ToBase();
            var workFlow = workflowRequest.Result;

            var newEndState = workFlow.States.FirstOrDefault(x => x.Id.Equals(stateId));
            if (newEndState == null) return new NotFoundResultModel<object>().ToBase();
            if (newEndState.IsEndState) return new SuccessResultModel<object>().ToBase();
            var response = new ResultModel();
            if (newEndState.IsEndState)
            {
                response.Errors.Add(new ErrorModel(nameof(WorkFlowErrorCodes.GRWF_0x106), WorkFlowErrorCodes.GRWF_0x106));
                return response;
            }

            var toUpdate = workFlow.States.Select(x =>
            {
                x.IsEndState = x.Id.Equals(stateId);
                return x;
            });

            _workFlowContext.States.UpdateRange(toUpdate);

            return await _workFlowContext.PushAsync();
        }

        /// <summary>
        /// Create transition
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResultModel<Guid>> CreateTransitionAsync([Required]Guid? workFlowId, AddTransitionViewModel model)
        {
            Arg.NotNull(model, nameof(AddTransitionViewModel));
            var response = new ResultModel<Guid>();
            var workflowRequest = await GetWorkFlowByIdAsync(workFlowId);
            if (!workflowRequest.IsSuccess) return workflowRequest.Map<Guid>();
            var workFlow = workflowRequest.Result;
            var fromState = workFlow.States.FirstOrDefault(x => x.Id.Equals(model.FromStateId));
            var toState = workFlow.States.FirstOrDefault(x => x.Id.Equals(model.ToStateId));
            if (fromState == null || toState == null)
            {
                response.Errors.Add(new ErrorModel(string.Empty, $"Workflow {workFlow.Name} does not have such states"));
                return response;
            }

            if (IsDuplicateTransition(workFlow, model.FromStateId, model.ToStateId))
            {
                response.Errors.Add(new ErrorModel(nameof(WorkFlowErrorCodes.GRWF_0x101), WorkFlowErrorCodes.GRWF_0x101));
                return response;
            }

            if (IsConnectionByItSelf(workFlow, model.FromStateId, model.ToStateId))
            {
                response.Errors.Add(new ErrorModel(nameof(WorkFlowErrorCodes.GRWF_0x100), WorkFlowErrorCodes.GRWF_0x100));
                return response;
            }

            var transition = WorkFlowMapper.Map(model, workFlowId);

            await _workFlowContext.Transitions.AddAsync(transition);
            var dbRequest = await _workFlowContext.PushAsync();
            return dbRequest.Map(transition.Id);
        }

        /// <summary>
        /// Get transition by id
        /// </summary>
        /// <param name="transitionId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Transition>> GetTransitionByIdAsync(Guid? transitionId)
        {
            if (transitionId == null) return new InvalidParametersResultModel<Transition>();
            var transition = await _workFlowContext.Transitions
                .AsNoTracking()
                .Include(x => x.FromState)
                .Include(x => x.ToState)
                .Include(x => x.TransitionRoles)
                .Include(x => x.TransitionActions)
                .ThenInclude(x => x.Action)
                .Include(x => x.WorkFlow)
                .FirstOrDefaultAsync(x => x.Id.Equals(transitionId));
            if (transition == null) return new NotFoundResultModel<Transition>();
            return new SuccessResultModel<Transition>(transition);
        }

        /// <summary>
        /// Get transition by start and end
        /// </summary>
        /// <param name="fromStateId"></param>
        /// <param name="toStateId"></param>
        /// <returns></returns>
        public async Task<ResultModel<Transition>> GetTransitionAsync([Required] Guid? fromStateId, [Required]Guid? toStateId)
        {
            if (fromStateId == null || toStateId == null) return new InvalidParametersResultModel<Transition>();
            var transition = await _workFlowContext.Transitions
                .Include(x => x.FromState)
                .Include(x => x.ToState)
                .Include(x => x.TransitionRoles)
                .Include(x => x.TransitionActions)
                .ThenInclude(x => x.Action)
                .Include(x => x.WorkFlow)
                .FirstOrDefaultAsync(x => x.FromStateId.Equals(fromStateId) && x.ToStateId.Equals(toStateId));
            if (transition == null) return new NotFoundResultModel<Transition>();
            return new SuccessResultModel<Transition>(transition);
        }

        /// <summary>
        /// Get state by id
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<StateGetViewModel>> GetStateByIdAsync(Guid? stateId)
        {
            if (stateId == null) return new InvalidParametersResultModel<StateGetViewModel>();
            var state = await _workFlowContext.States
                .Include(x => x.WorkFlow)
                .FirstOrDefaultAsync(x => x.Id.Equals(stateId));
            if (state == null) return new NotFoundResultModel<StateGetViewModel>();
            return new SuccessResultModel<StateGetViewModel>(WorkFlowMapper.Map(state));
        }

        /// <summary>
        /// Remove state by id
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> RemoveStateAsync(Guid? stateId)
        {
            var response = new ResultModel();
            if (stateId == null) return new InvalidParametersResultModel();
            var state = await _workFlowContext.States
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(stateId));
            if (state == null) return new NotFoundResultModel();
            if (await IsUsedStateAsync(stateId))
            {
                response.Errors.Add(new ErrorModel(nameof(WorkFlowErrorCodes.GRWF_0x102), WorkFlowErrorCodes.GRWF_0x102));
                return response;
            }

            _workFlowContext.States.Remove(state);
            return await _workFlowContext.PushAsync();
        }

        /// <summary>
        /// Remove workflow
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> RemoveWorkFlowAsync([Required]Guid? workFlowId)
        {
            var response = new ResultModel();
            var workflowRequest = await GetWorkFlowByIdAsync(workFlowId);
            if (!workflowRequest.IsSuccess) return workflowRequest.ToBase();
            var workFlow = workflowRequest.Result;
            if (await IsUsedWorkflowAsync(workFlowId))
            {
                response.Errors.Add(new ErrorModel(nameof(WorkFlowErrorCodes.GRWF_0x103), WorkFlowErrorCodes.GRWF_0x103));
                return response;
            }

            _workFlowContext.WorkFlows.Remove(workFlow);
            return await _workFlowContext.PushAsync();
        }

        /// <summary>
        /// Remove transition by id
        /// </summary>
        /// <param name="transitionId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> RemoveTransitionByIdAsync(Guid? transitionId)
        {
            if (transitionId == null) return new InvalidParametersResultModel<object>().ToBase();
            var transition = await _workFlowContext.Transitions
                .FirstOrDefaultAsync(x => x.Id.Equals(transitionId));
            if (transition == null) return new NotFoundResultModel<object>().ToBase();
            _workFlowContext.Transitions.Remove(transition);

            return await _workFlowContext.PushAsync();
        }


        /// <summary>
        /// Add or update allowed roles to transition
        /// </summary>
        /// <param name="transitionId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AddOrUpdateTransitionAllowedRolesAsync([Required]Guid? transitionId, IEnumerable<Guid> roles)
        {
            var transitionRequest = await GetTransitionByIdAsync(transitionId);
            if (!transitionRequest.IsSuccess) return transitionRequest.ToBase();
            var transition = transitionRequest.Result;
            var newRoles = (await _userManager.FilterValidRolesAsync(roles)).ToList();
            var oldRoles = transition.TransitionRoles.Select(x => x.RoleId).ToList();

            var (removeItems, addItems) = oldRoles.GetDifferences(newRoles);

            if (removeItems.Any())
            {
                var mappedRoles = removeItems.Select(x =>
                {
                    return transition.TransitionRoles.FirstOrDefault(y => y.RoleId.Equals(x));
                }).ToList();
                _workFlowContext.TransitionRoles.RemoveRange(mappedRoles);
            }

            if (addItems.Any())
            {
                var mappedRoles = addItems.Select(x => new TransitionRole
                {
                    TransitionId = transition.Id,
                    RoleId = x
                });
                await _workFlowContext.TransitionRoles.AddRangeAsync(mappedRoles);
            }
            return await _workFlowContext.PushAsync();
        }

        /// <summary>
        /// Get actions
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<WorkflowAction>>> GetAllRegisteredActionsAsync()
            => new SuccessResultModel<IEnumerable<WorkflowAction>>(await _workFlowContext.WorkflowActions.ToListAsync());

        /// <summary>
        /// Update transition name
        /// </summary>
        /// <param name="transitionId"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public async Task<ResultModel> UpdateTransitionNameAsync(Guid? transitionId, string newName)
        {
            if (transitionId == null) return new InvalidParametersResultModel();
            var transition = await _workFlowContext.Transitions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(transitionId));
            if (transition == null) return new NotFoundResultModel();
            transition.Name = newName;
            _workFlowContext.Transitions.Update(transition);
            return await _workFlowContext.PushAsync();
        }

        /// <summary>
        /// Update state metadata
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="newName"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public async Task<ResultModel> UpdateStateGeneralInfoAsync(Guid? stateId, string newName, string description)
        {
            if (stateId == null || newName.IsNullOrEmpty()) return new InvalidParametersResultModel();
            var state = await _workFlowContext.States
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(stateId));
            if (state == null) return new NotFoundResultModel();
            state.Name = newName;
            state.Description = description;
            _workFlowContext.States.Update(state);
            return await _workFlowContext.PushAsync();
        }

        /// <summary>
        /// Add or update transition actions
        /// </summary>
        /// <param name="transitionId"></param>
        /// <param name="actionHandlers"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AddOrUpdateTransitionActionsAsync([Required]Guid? transitionId, IEnumerable<Guid> actionHandlers)
        {
            var transitionRequest = await GetTransitionByIdAsync(transitionId);
            if (!transitionRequest.IsSuccess) return transitionRequest.ToBase();
            var transition = transitionRequest.Result;
            var oldActions = transition.TransitionActions.Select(x => x.ActionId).ToList();
            var newActions = actionHandlers.ToList();
            var (toRemove, toAdd) = oldActions.GetDifferences(newActions);
            if (toRemove.Any())
            {
                var mappedActions = transition.TransitionActions.Where(x => toRemove.Contains(x.ActionId)).ToList();
                _workFlowContext.TransitionActions.RemoveRange(mappedActions);
            }

            if (toAdd.Any())
            {
                var mappedActions = toAdd.Select(x => new TransitionAction
                {
                    TransitionId = transition.Id,
                    ActionId = x
                });
                await _workFlowContext.TransitionActions.AddRangeAsync(mappedActions);
            }

            return await _workFlowContext.PushAsync();
        }

        /// <summary>
        /// Get workflow states
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<StateGetViewModel>>> GetWorkFlowStatesAsync([Required] Guid? workFlowId)
        {
            var workFlowRequest = await GetWorkFlowByIdAsync(workFlowId);
            return !workFlowRequest.IsSuccess ? workFlowRequest.Map<IEnumerable<StateGetViewModel>>()
                : new SuccessResultModel<IEnumerable<StateGetViewModel>>(WorkFlowMapper.Map(workFlowRequest.Result.States));
        }

        /// <summary>
        /// Get all workflows
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<GetWorkFlowViewModel>>> GetAllWorkFlowsAsync()
        {
            var workFlows = await _workFlowContext.WorkFlows.AsNoTracking().ToListAsync();
            return new SuccessResultModel<IEnumerable<GetWorkFlowViewModel>>(WorkFlowMapper.Map(workFlows));
        }


        #region Helpers

        /// <summary>
        /// Check for duplicate transitions
        /// </summary>
        /// <param name="workFlow"></param>
        /// <param name="startState"></param>
        /// <param name="endState"></param>
        /// <returns></returns>
        protected static bool IsDuplicateTransition(WorkFlow workFlow, Guid? startState, Guid? endState)
            => workFlow?.Transitions?.Any(x => x.FromStateId.Equals(startState) && x.ToStateId.Equals(endState)) ?? false;

        /// <summary>
        /// Is connection by it self
        /// </summary>
        /// <param name="workFlow"></param>
        /// <param name="startState"></param>
        /// <param name="endState"></param>
        /// <returns></returns>
        protected static bool IsConnectionByItSelf(WorkFlow workFlow, Guid? startState, Guid? endState)
            => startState == endState;

        /// <summary>
        /// Get workflow by id
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        protected virtual async Task<ResultModel<WorkFlow>> GetWorkFlowWhitOutIncludesAsync([Required] Guid? workFlowId)
        {
            if (workFlowId == null) return new InvalidParametersResultModel<WorkFlow>();
            var workFlow = await _workFlowContext.WorkFlows
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(workFlowId));
            if (workFlow == null) return new NotFoundResultModel<WorkFlow>();
            return new SuccessResultModel<WorkFlow>(workFlow);
        }

        /// <summary>
        /// Check if state is used
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        protected virtual async Task<bool> IsUsedStateAsync([Required] Guid? stateId)
            => await _workFlowContext.EntryStates
                .AsNoTracking()
                .Include(x => x.State)
                .AnyAsync(x => x.StateId.Equals(stateId));

        /// <summary>
        /// Check if is any registered contract to workflow
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        protected virtual async Task<bool> IsUsedWorkflowAsync([Required] Guid? workFlowId)
            => await _workFlowContext.Contracts
                .AsNoTracking()
                .Include(x => x.WorkFlow)
                .AnyAsync(x => x.WorkFlowId.Equals(workFlowId));

        #endregion
    }
}