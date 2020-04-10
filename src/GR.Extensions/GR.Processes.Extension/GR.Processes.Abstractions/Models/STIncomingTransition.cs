using System;

namespace GR.Processes.Abstractions.Models
{
    public class STIncomingTransition
    {
        public Guid ProcessTransitionId { get; set; }

        public STProcessTransition IncomingTransition { get; set; }
        public Guid IncomingTransitionId { get; set; }
    }
}
