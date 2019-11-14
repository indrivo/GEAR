using GR.Procesess.Models;
using GR.Procesess.Models.ParserModels;
using GR.Procesess.ViewModels;
using System.Collections.Generic;

namespace GR.Procesess.Abstraction
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
