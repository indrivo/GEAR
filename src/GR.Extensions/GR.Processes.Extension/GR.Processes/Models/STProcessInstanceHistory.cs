using System;
using System.ComponentModel;

namespace GR.Procesess.Models
{
    public class STProcessInstanceHistory
    {
        /// <summary>
        /// Process instance
        /// </summary>
        public STProcessInstance ProcessInstance { get; set; }
        public Guid ProcessInstanceId { get; set; }

        /// <summary>
        /// Process transition
        /// </summary>
        public STProcessTransition ProcessTransition { get; set; }
        public Guid ProcessTransitionId { get; set; }

        /// <summary>
        /// Transition state 
        /// </summary>
        [DefaultValue(TransitionState.Inactive)]
        public TransitionState TransitionState { get; set; }
    }
    public enum TransitionState
    {
        Completed, Started, Canceled, InProgress, Inactive
    }
}
