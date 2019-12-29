using System;
using System.Collections.Generic;
using System.Linq;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.WorkFlows.Abstractions.Models;
using GR.WorkFlows.Abstractions.ViewModels;

namespace GR.WorkFlows.Abstractions.Helpers
{
    [Author(Authors.LUPEI_NICOLAE, 1.1, "Add mappers")]
    public static class WorkFlowMapper
    {
        /// <summary>
        /// Map add new state to state entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static State Map(AddNewStateViewModel model)
        {
            Arg.NotNull(model, nameof(AddNewStateViewModel));
            return new State
            {
                Name = model.Name,
                WorkFlowId = model.WorkFlowId,
                Description = model.Description,
                AdditionalSettings = model.AdditionalSettings.SerializeAsJson()
            };
        }

        /// <summary>
        /// Map new workflow
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static WorkFlow Map(AddNewWorkflowViewModel model)
        {
            Arg.NotNull(model, nameof(AddNewWorkflowViewModel));

            return new WorkFlow
            {
                Name = model.Name,
                Description = model.Description,
                Enabled = model.Enabled
            };
        }

        /// <summary>
        /// Map
        /// </summary>
        /// <param name="model"></param>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        public static Transition Map(AddTransitionViewModel model, Guid? workFlowId)
        {
            Arg.NotNull(model, nameof(AddTransitionViewModel));
            Arg.NotNull(workFlowId, nameof(Map));

            return new Transition
            {
                Name = model.Name,
                FromStateId = model.FromStateId,
                ToStateId = model.ToStateId,
                WorkflowId = workFlowId.GetValueOrDefault()
            };
        }

        /// <summary>
        /// Get list for get response
        /// </summary>
        /// <param name="workFlows"></param>
        /// <returns></returns>
        public static IEnumerable<GetWorkFlowViewModel> Map(IEnumerable<WorkFlow> workFlows)
            => workFlows.Select(x => new GetWorkFlowViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Enabled = x.Enabled
            }).ToList();

        /// <summary>
        /// Map get
        /// </summary>
        /// <param name="states"></param>
        /// <returns></returns>
        public static IEnumerable<StateGetViewModel> Map(IEnumerable<State> states)
             => states?.Select(Map);

        /// <summary>
        /// Map state to view model
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static StateGetViewModel Map(State state)
        {
            Arg.NotNull(state, nameof(Map));
            return new StateGetViewModel
            {
                Id = state.Id,
                Name = state.Name,
                WorkFlowId = state.WorkFlowId,
                Description = state.Description,
                IsStartState = state.IsStartState,
                IsEndState = state.IsEndState,
                AdditionalSettings = state.AdditionalSettings.Deserialize<Dictionary<string, string>>()
            };
        }

        /// <summary>
        /// Map transitions
        /// </summary>
        /// <param name="transitions"></param>
        /// <returns></returns>
        public static IEnumerable<TransitionGetViewModel> Map(IEnumerable<Transition> transitions)
            => transitions?.Select(transition => new TransitionGetViewModel
            {
                Id = transition.Id,
                Name = transition.Name,
                WorkFlow = transition.WorkFlow,
                FromState = transition.FromState,
                ToState = transition.ToState
            });

        /// <summary>
        /// Map get
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static WorkFlowGetViewModel Map(WorkFlow model)
        {
            Arg.NotNull(model, nameof(Map));
            return new WorkFlowGetViewModel
            {
                Name = model.Name,
                Description = model.Description,
                Enabled = model.Enabled,
                States = Map(model.States),
                Transitions = Map(model.Transitions)
            };
        }

        /// <summary>
        /// Map
        /// </summary>
        /// <param name="histories"></param>
        /// <returns></returns>
        public static IEnumerable<EntryHistoryViewModel> Map(IEnumerable<EntryStateHistory> histories)
            => histories.Select(x => new EntryHistoryViewModel
            {
                ToState = x.ToState,
                FromState = x.FromState,
                Author = x.Author,
                Message = x.Message,
                EntryId = x.EntryState.EntryId
            });
    }
}
