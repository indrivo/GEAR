using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.WorkFlows.Abstractions.Models;

namespace GR.WorkFlows.Abstractions.Helpers.ActionHandlers
{
    public abstract class BaseWorkFlowAction
    {
        #region Injectable

        /// <summary>
        /// Executor
        /// </summary>
        protected readonly IWorkFlowExecutorService Executor;

        #endregion

        /// <summary>
        /// Current transition
        /// </summary>
        protected Transition CurrentTransition { get; set; }

        /// <summary>
        /// Next transitions
        /// </summary>
        protected IEnumerable<Transition> NextTransitions { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="currentTransition"></param>
        /// <param name="nextTransitions"></param>
        protected BaseWorkFlowAction(Transition currentTransition, IEnumerable<Transition> nextTransitions)
        {
            CurrentTransition = currentTransition;
            NextTransitions = nextTransitions;
            Executor = IoC.Resolve<IWorkFlowExecutorService>();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        public abstract Task InvokeExecuteAsync(Dictionary<string, string> data);
    }
}
