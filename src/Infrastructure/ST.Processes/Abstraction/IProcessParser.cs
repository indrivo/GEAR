using ST.Procesess.Models;
using ST.Procesess.Models.ParserModels;
using ST.Procesess.ViewModels;
using System.Collections.Generic;

namespace ST.Procesess.Abstraction
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
