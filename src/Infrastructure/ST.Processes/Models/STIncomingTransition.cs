using System;

namespace ST.Procesess.Models
{
    public class STIncomingTransition
    {
        public Guid ProcessTransitionId { get; set; }

        public STProcessTransition IncomingTransition { get; set; }
        public Guid IncomingTransitionId { get; set; }
    }
}
