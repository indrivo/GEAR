using System.Collections.Generic;
using GR.Processes.Abstractions.Models;

namespace GR.Processes.Abstractions.ViewModels
{
    public class STParsedProcess
    {
        public STProcess Process { get; set; }
        public IEnumerable<STIncomingTransition> IncomingTransitions { get; set; }
        public IEnumerable<STOutgoingTransition> OutgoingTransitions { get; set; }
    }
}
