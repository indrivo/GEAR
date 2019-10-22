using GR.Procesess.Models;
using System.Collections.Generic;

namespace GR.Procesess.ViewModels
{
    public class STParsedProcess
    {
        public STProcess Process { get; set; }
        public IEnumerable<STIncomingTransition> IncomingTransitions { get; set; }
        public IEnumerable<STOutgoingTransition> OutgoingTransitions { get; set; }
    }
}
