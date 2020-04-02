using System.Collections.Generic;
using GR.Processes.Abstractions.Models;
using GR.Processes.Abstractions.Models.ParserModels;

namespace GR.Processes.Abstractions
{
    public interface IProcessParser
    {
        void Init(string schema, IEnumerable<Dictionary<string, string>> xSettings);
        string GetStringSchema { get; }
        XSchema GetXSchema { get; }
        IEnumerable<STProcess> GetProcesses();
        IEnumerable<STProcess> GetProcessFromCollaborationDiagram();
    }
}
