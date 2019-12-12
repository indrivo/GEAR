using System;

namespace GR.Procesess.Models
{
    public class STOutgoingTransition
    {
        public Guid ProcessTransitionId { get; set; }

        public STProcessTransition OutgoingTransition { get; set; }
        public Guid OutgoingTransitionId { get; set; }
    }
}
