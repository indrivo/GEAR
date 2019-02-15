using Mapster;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using ST.Identity.Data.Permissions;
using ST.Procesess.Abstraction;
using ST.Procesess.Extensions;
using ST.Procesess.Models;
using ST.Procesess.Models.ParserModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ST.Procesess.Parsers
{
    public class ProcessParser : IProcessParser
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        /// <summary>
        /// Xml as string
        /// </summary>
        private string Schema { get; set; }
        /// <summary>
        /// Xml parsed as XSchema
        /// </summary>
        private XSchema XSchema { get; set; }
        private IEnumerable<Dictionary<string, string>> XSettings { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="roleManager"></param>
        public ProcessParser(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        /// <summary>
        /// Init parser
        /// </summary>
        /// <param name="schema"></param>
        public void Init(string schema, IEnumerable<Dictionary<string, string>> xSettings)
        {
            Schema = schema;
            var parsed = BPMN.Model.Parse(schema);
            XSchema = parsed.Adapt<XSchema>();
            XSettings = xSettings;
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
        /// 
        /// </summary>
        private void ValidateInit()
        {
            if (string.IsNullOrEmpty(Schema) || XSchema == null)
                throw new Exception("Need to be called Init function because Schema is null");
        }

        /// <summary>
        /// Get processes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<STProcess> GetProcesses()
        {
            ValidateInit();
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
            ValidateInit();
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

                        //Initiate new process
                        var process = new STProcess
                        {
                            Name = xProcess.ID,
                            ProcessSettings = processSettings.ToStringSettings(),
                            ProcessTransitions = new List<STProcessTransition>()
                        };

                        var xActors = XSchema.GetActorsFromProcess(xProcess);
                        foreach (var xActor in xActors)
                        {
                            var actorSettings = XSettings.FirstOrDefault(x => x?["id"] == xActor.ID);
                            if (!actorSettings.ContainsKey("name")) continue;
                            var role = _roleManager.Roles.FirstOrDefault(x => x.Name == actorSettings["name"]);
                            if (role == null) continue;

                            //Initiate new actor
                            var actor = new STTransitionActor
                            {
                                Name = xActor.ID,
                                RoleId = Guid.Parse(role.Id),
                                ActorSettings = actorSettings.ToStringSettings()
                            };
                            //Get actor transitions
                            var xReferences = xActor.Elements?["flowNodeRef"];
                            foreach (var xReference in xReferences)
                            {
                                var xEl = XSchema.Elements.FirstOrDefault(x => x.ID == xReference.Attributes.FirstOrDefault().Value);
                                if (xEl == null) continue;

                                var transition = new STProcessTransition
                                {
                                    Name = xEl.ID,
                                    Process = process,
                                    TransitionActors = new List<STTransitionActor> { actor },
                                    TransitionType = xEl.TypeName.ToUpperFirstString().GetTransitionType()
                                };

                                var incomings = xEl.Elements["incoming"];


                                var (incomingExist, incomingTransition) = CheckExistentIncoming(process, "");

                                if (incomingExist)
                                {
                                    transition.ProcessTransition = incomingTransition;
                                }

                                process.ProcessTransitions.Add(transition);
                            }
                        }
                        processes.Add(process);
                    }
                }
            }
            return processes;
        }

        /// <summary>
        /// Check incoming
        /// </summary>
        /// <param name="process"></param>
        /// <param name="elementId"></param>
        /// <returns></returns>
        private (bool, STProcessTransition) CheckExistentIncoming(STProcess process, string elementId)
        {
            if (!process.ProcessTransitions.Any()) return default;
            var check = process.ProcessTransitions.FirstOrDefault(x => x.Name == elementId);
            if (check == null) return default;
            return (true, check);
        }
    }
}
