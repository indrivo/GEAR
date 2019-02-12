using Mapster;
using Newtonsoft.Json;
using ST.Procesess.Abstraction;
using ST.Procesess.Models;
using ST.Procesess.Models.ParserModels;
using System.Collections.Generic;
using System.Linq;

namespace ST.Procesess.Parsers
{
    public class ProcessParser : IProcessParser
    {
        private string Schema { get; set; }
        private XSchema XSchema { get; set; }
        private IEnumerable<Dictionary<string, string>> XSettings { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="schema"></param>
        public ProcessParser(string schema, IEnumerable<Dictionary<string, string>> xSettings)
        {
            Schema = schema;
            var parsed = BPMN.Model.Parse(schema);
            XSchema = parsed.Adapt<XSchema>();
            XSettings = xSettings;
        }

        public ProcessParser(string schema)
        {
            Schema = schema;
            var parsed = BPMN.Model.Parse(schema);
            XSchema = parsed.Adapt<XSchema>();
        }

        /// <summary>
        /// Get string Schema
        /// </summary>
        /// <returns></returns>
        public string GetStringSchema
        {
            get => Schema;
        }

        /// <summary>
        /// Get Schema
        /// </summary>
        public XSchema GetXSchema
        {
            get => XSchema;
        }

        /// <summary>
        /// Get processes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<STProcess> GetProcesses()
        {
            var processes = new List<STProcess>();
            if (XSchema == null) return default;
            var json = JsonConvert.SerializeObject(XSchema);

            if (XSchema.IsCollaborationDiagram())
            {
                processes = GetProcessFromCollaborationDiagram().ToList();
            }
            else
            {

            }

            return processes;
        }

        /// <summary>
        /// Get processes if is a collaboration diagram
        /// </summary>
        /// <returns></returns>
        public IEnumerable<STProcess> GetProcessFromCollaborationDiagram()
        {
            var processes = new List<STProcess>();
            var xColaborations = XSchema.GetCollaborations();

            if (!xColaborations.Any()) return default;
            foreach (var xCollaboration in xColaborations)
            {
                var xParticipants = XSchema.GetParticipantsbyCollaborationId(xCollaboration.ID);
                foreach (var xParticipant in xParticipants)
                {
                    var xProcess = XSchema.GetProcessFromParticipant(xParticipant);
                    if (xProcess != null)
                    {
                        var processSettings = XSettings.FirstOrDefault(x => x["id"] == xProcess.ID);
                        var xActors = XSchema.GetActorsFromProcess(xProcess);
                        foreach(var xActor in xActors)
                        {
                            var actorSettings = XSettings.FirstOrDefault(x => x["id"] == xActor.ID);
                            var actorName = actorSettings?["name"];
                        }
                    }
                }
            }
            return processes;
        }
    }
}
