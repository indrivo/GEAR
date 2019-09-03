using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ST.Audit.Abstractions.Attributes;
using ST.Audit.Abstractions.Enums;
using ST.Core;

namespace ST.Procesess.Models
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class STProcessTransition : BaseModel
    {
        /// <summary>
        /// Name of transition
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Transition type
        /// </summary>
        [Required]
        public TransitionType TransitionType { get; set; }

        /// <summary>
        /// Reference to process 
        /// </summary>
        public STProcess Process { get; set; }
        public Guid ProcessId { get; set; }

        /// <summary>
        /// Settings defined by author
        /// </summary>
        public string TransitionSettings { get; set; }

        /// <summary>
        /// Actors who can start this transition
        /// </summary>
        public IEnumerable<STTransitionActor> TransitionActors { get; set; }

        /// <summary>
        /// Incoming transitions
        /// </summary>
        public IList<STIncomingTransition> IncomingTransitions { get; set; } = new List<STIncomingTransition>();

        /// <summary>
        /// Outgoing transitions
        /// </summary>
        public IList<STOutgoingTransition> OutgoingTransitions { get; set; } = new List<STOutgoingTransition>();
    }

    public enum TransitionType
    {
        SequenceFlow,
        Process,
        Lane,
        StartEvent,
        EndEvent,
        IntermediateThrowEvent,
        SendTask,
        ReceiveTask,
        UserTask,
        ManualTask,
        BusinessRuleTask,
        ServiceTask,
        ScriptTask,
        CallActivity,
        SubProcess,
        Transaction,
        DataStoreReference,
        Participant,
        Collaboration
    }
}
