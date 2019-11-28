using System;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.WorkFlows.Abstractions.Models;
using GR.WorkFlows.Abstractions.ViewModels;

namespace GR.WorkFlows.Abstractions.Helpers
{
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
    }
}
