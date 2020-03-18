using System;

namespace GR.Processes.Abstractions.Models
{
    public class STOutgoingTransition
    {
        public Guid ProcessTransitionId { get; set; }

        public STProcessTransition OutgoingTransition { get; set; }
        public Guid OutgoingTransitionId { get; set; }
    }
}
