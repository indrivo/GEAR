using Mapster;
using Microsoft.AspNetCore.Identity;
using ST.Identity.Data.Permissions;
using ST.Procesess.Abstraction;
using ST.Procesess.Extensions;
using ST.Procesess.Models;
using ST.Procesess.Models.ParserModels;
using ST.Procesess.ViewModels;
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
        /// <param name="xSettings"></param>
        public void Init(string schema, IEnumerable<Dictionary<string, string>> xSettings)
        {
            Schema = schema;
            if (!string.IsNullOrEmpty(schema))
            {
                var parsed = BPMN.Model.Parse(schema);
                XSchema = parsed.Adapt<XSchema>();
            }
            else
            {
                throw new Exception("Invalid schema content!!!");
            }

            XSettings = xSettings;
        }

        /// <summary>
        /// Get string Schema
        /// </summary>
        /// <returns></returns>
        public string GetStringSchema => Schema;

        /// <summary>
        /// Get Schema
        /// </summary>
        public XSchema GetXSchema => XSchema;

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
            if (XSchema == null) return processes;

            if (XSchema.IsCollaborationDiagram())
            {
                processes = GetProcessFromCollaborationDiagram().ToList();
            }
            else
            {
                var startEvents = XSchema.Elements.Where(x => x.TypeName.ToUpperFirstString().GetTransitionType() == TransitionType.StartEvent);
                if (!startEvents.Any()) return processes;
                {
                    var sequences = XSchema.Elements.Where(x =>
                        x.TypeName.ToUpperFirstString().GetTransitionType() == TransitionType.SequenceFlow).ToList();

                    if (!sequences.Any()) return processes;
                    {
                        var prElement = XSchema.Elements.FirstOrDefault(x => x.TypeName.ToUpperFirstString().GetTransitionType() == TransitionType.Process);
                        var process = new STProcess
                        {
                            Name = prElement?.ID,
                            ProcessTransitions = new List<STProcessTransition>(),
                            ProcessSettings = XSettings.FirstOrDefault(x => x?["id"] == prElement?.ID).ToStringSettings(),
                            Created = DateTime.Now,
                            Changed = DateTime.Now
                        };
                        foreach (var sequence in sequences)
                        {
                            var source = sequence.Attributes["sourceRef"];
                            var target = sequence.Attributes["targetRef"];

                            //Check source
                            if (string.IsNullOrEmpty(source)) continue;
                            //Check if source exist
                            var sourceTransition = process.ProcessTransitions.FirstOrDefault(x => x.Name == source);
                            if (sourceTransition == null)
                            {
                                var sourceElement = XSchema.Elements.FirstOrDefault(x => x.ID == source);
                                if (sourceElement == null) continue;
                                {
                                    var newSourceTran = new STProcessTransition
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = source,
                                        TransitionSettings = XSettings.FirstOrDefault(x => x?["id"] == source).ToStringSettings(),
                                        TransitionType = sourceElement.TypeName.ToUpperFirstString().GetTransitionType()
                                    };

                                    if (string.IsNullOrEmpty(target)) continue;
                                    {
                                        //Check if target exist
                                        var trans = process.ProcessTransitions.FirstOrDefault(x => x.Name == target);

                                        //Check if exist transition has Incoming transition
                                        if (trans != null)
                                        {
                                            var existIncoming = trans.IncomingTransitions.FirstOrDefault(x => x.IncomingTransitionId == newSourceTran.Id);
                                            if (existIncoming == null)
                                            {
                                                process.ProcessTransitions[process.ProcessTransitions.IndexOf(trans)].IncomingTransitions.Add(new STIncomingTransition
                                                {
                                                    IncomingTransitionId = newSourceTran.Id,
                                                    ProcessTransitionId = trans.Id
                                                });
                                            }
                                        }
                                        //Create new target transition
                                        else
                                        {
                                            var targetElement = XSchema.Elements.FirstOrDefault(x => x.ID == target);
                                            if (targetElement != null)
                                            {
                                                var newSourceTran1 = new STProcessTransition
                                                {
                                                    Id = Guid.NewGuid(),
                                                    Name = target,
                                                    TransitionType = targetElement.TypeName.ToUpperFirstString().GetTransitionType(),
                                                    TransitionSettings = XSettings.FirstOrDefault(x => x?["id"] == target).ToStringSettings(),
                                                    IncomingTransitions = new List<STIncomingTransition> {
                                                        new STIncomingTransition {
                                                            IncomingTransitionId = newSourceTran.Id,
                                                            ProcessTransitionId = trans.Id
                                                        }
                                                    }
                                                };
                                                process.ProcessTransitions.Add(newSourceTran1);
                                            }
                                        }
                                    }
                                }
                            }
                            //Check if transition has outgoing
                            else
                            {
                                if (string.IsNullOrEmpty(target)) continue;
                                //Check outgoing transition
                                var outTransition = process.ProcessTransitions.FirstOrDefault(x => x.Name == target);
                                if (outTransition == null) continue;
                                {
                                    var exist = sourceTransition.OutgoingTransitions.FirstOrDefault(x => x.OutgoingTransitionId == outTransition.Id);
                                    if (exist == null)
                                    {
                                        sourceTransition.OutgoingTransitions.Add(new STOutgoingTransition
                                        {
                                            ProcessTransitionId = sourceTransition.Id,
                                            OutgoingTransitionId = outTransition.Id
                                        });
                                    }
                                }
                            }
                        }
                        processes.Add(process);
                    }
                }
            }

            return processes;
        }

        /// <summary>
        /// Get process from a simple diagram
        /// </summary>
        /// <returns></returns>
        private STParsedProcess GetProcessFromSimpleDiagram(IEnumerable<XElement> startEvents)
        {
            var xElements = startEvents.ToList();
            if (!xElements.Any()) return null;
            var process = new STParsedProcess();

            foreach (var startEvent in xElements)
            {
                //Initiate start transition
                var startTransition = new STProcessTransition
                {
                    Name = startEvent.ID,
                    TransitionType = startEvent.TypeName.ToUpperFirstString().GetTransitionType(),
                    TransitionSettings = XSettings.FirstOrDefault(x => x?["id"] == startEvent.ID).ToStringSettings()
                };

                var xOutgoings = startEvent.Elements["outgoing"];
                if (!xOutgoings.Any()) continue;
                var nextTransitions = GetOutgoingXElementTransitions(xOutgoings).ToList();
                startTransition.OutgoingTransitions = ConvertXElementToXComing<STOutgoingTransition>(xOutgoings, startTransition);
                var go = xOutgoings.Any();
                while (go)
                {
                    var (nextTrans, recycle) = GetNextTransitions(nextTransitions);

                    foreach (var nextTransition in nextTrans)
                    {
                        //TODO: Parse simple diagram
                    }

                    go = recycle;
                }
            }

            return process;
        }

        private (IEnumerable<STProcessTransition>, bool) GetNextTransitions(IEnumerable<XElement> elements)
        {
            return default;
        }

        private List<TGoing> ConvertXElementToXComing<TGoing>(IEnumerable<XElement> goings, STProcessTransition source) where TGoing : class
        {
            if (!goings.Any()) return default;
            if (typeof(TGoing) == typeof(STIncomingTransition))
            {
                var data = new List<STIncomingTransition>();


                return data.Adapt<List<TGoing>>();
            }
            else if (typeof(TGoing) == typeof(STOutgoingTransition))
            {
                var data = new List<STOutgoingTransition>();


                return data.Adapt<List<TGoing>>();
            }
            return default;
        }

        /// <summary>
        /// Get Outgoing transitions from xElement outgoings
        /// </summary>
        /// <param name="outgoings"></param>
        /// <returns></returns>
        private IEnumerable<XElement> GetOutgoingXElementTransitions(IEnumerable<XElement> outgoings)
        {
            var transitions = new List<XElement>();
            foreach (var outgoing in outgoings)
            {
                var sequency = XSchema.Elements.FirstOrDefault(x => x.ID == outgoing.Attributes.FirstOrDefault().Value);
                var transition = GetXElementFromTargetSequency(sequency);
                if (transition != null)
                    transitions.Add(transition);
            }

            return transitions;
        }

        /// <summary>
        /// Get XElement from sequency
        /// </summary>
        /// <param name="sequency"></param>
        /// <returns></returns>
        private XElement GetXElementFromTargetSequency(XElement sequency)
        {
            if (sequency == null) return null;
            var targetRefId = sequency.Attributes["targetRef"];
            return XSchema.Elements.FirstOrDefault(x => x.ID == targetRefId);
        }

        /// <summary>
        /// Get source XElement from sequency
        /// </summary>
        /// <param name="sequency"></param>
        /// <returns></returns>
        private XElement GetXElementFromSourceSequency(XElement sequency)
        {
            if (sequency == null) return null;
            var targetRefId = sequency.Attributes["sourceRef"];
            return XSchema.Elements.FirstOrDefault(x => x.ID == targetRefId);
        }


        /// <summary>
        /// Get processes if is a collaboration diagram
        /// </summary>
        /// <returns></returns>
        public IEnumerable<STProcess> GetProcessFromCollaborationDiagram()
        {
            ValidateInit();
            var processes = new List<STProcess>();
            var xCollaborations = XSchema.GetCollaborations();

            var xElements = xCollaborations.ToList();
            if (!xElements.Any()) return default;
            foreach (var xCollaboration in xElements)
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
                            ProcessTransitions = new List<STProcessTransition>(),
                            Created = DateTime.Now,
                            Changed = DateTime.Now
                        };

                        var incomingMap = new List<KeyValuePair<string, string>>();
                        var outgoingMap = new List<KeyValuePair<string, string>>();

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
                            if (xReferences == null) continue;
                            {
                                foreach (var xReference in xReferences)
                                {
                                    var xEl = XSchema.Elements.FirstOrDefault(x =>
                                        x.ID == xReference.Attributes.FirstOrDefault().Value);
                                    if (xEl == null) continue;
                                    var transitionSettings = XSettings.FirstOrDefault(x => x?["id"] == xEl.ID);
                                    var transition = new STProcessTransition
                                    {
                                        Name = xEl.ID,
                                        Process = process,
                                        TransitionActors = new List<STTransitionActor> { actor },
                                        TransitionType = xEl.TypeName.ToUpperFirstString().GetTransitionType(),
                                        TransitionSettings = transitionSettings.ToStringSettings()
                                    };

                                    var xIncoming = xEl.Elements["incoming"];
                                    var xOutgoing = xEl.Elements["outgoing"];
                                    var inc = ExtractIncomingTransitions(xIncoming, transition.Name);
                                    if (inc != null)
                                        incomingMap.AddRange(inc);
                                    var outg = ExtractOutGoingTransitions(xOutgoing, transition.Name);
                                    if (outg != null)
                                        outgoingMap.AddRange(outg);
                                    process.ProcessTransitions.Add(transition);
                                }
                            }
                        }

                        process.ProcessTransitions = MapTransitions(process.ProcessTransitions.ToList(), incomingMap, outgoingMap);

                        processes.Add(process);
                    }
                }
            }
            return processes;
        }

        /// <summary>
        /// Map in and out transitions
        /// </summary>
        /// <param name="transitions"></param>
        /// <param name="incoming"></param>
        /// <param name="outgoing"></param>
        /// <returns></returns>
        private List<STProcessTransition> MapTransitions(
           List<STProcessTransition> transitions, IEnumerable<KeyValuePair<string, string>> incoming, IEnumerable<KeyValuePair<string, string>> outgoing)
        {
            if (!transitions.Any()) return default;

            foreach (var transition in transitions)
            {
                var keyValuePairs = incoming?.ToList();
                var tIncoming = keyValuePairs?.Where(x => x.Key == transition.Name).ToList();
                var tOutgoings = outgoing?.Where(x => x.Key == transition.Name).ToList();

                //Incoming
                if (keyValuePairs != null && keyValuePairs.Any())
                {
                    foreach (var (key, value) in tIncoming)
                    {
                        var target = transitions.FirstOrDefault(x => x.Name == key);
                        var source = transitions.FirstOrDefault(x => x.Name == value);

                        if (target != null && source != null)
                        {
                            transition.IncomingTransitions.Add(new STIncomingTransition
                            {
                                ProcessTransitionId = target.Id,
                                IncomingTransition = source,
                                IncomingTransitionId = source.Id
                            });
                        }
                    }
                }

                //Outgoing
                if (outgoing.Any())
                {
                    if (tOutgoings != null)
                        foreach (var outg in tOutgoings)
                        {
                            var source = transitions.FirstOrDefault(x => x.Name == outg.Key);
                            var target = transitions.FirstOrDefault(x => x.Name == outg.Value);

                            if (target != null && source != null)
                            {
                                transition.OutgoingTransitions.Add(new STOutgoingTransition
                                {
                                    ProcessTransitionId = source.Id,
                                    OutgoingTransition = target,
                                    OutgoingTransitionId = target.Id
                                });
                            }
                        }
                }
            }

            return transitions;
        }

        /// <summary>
        /// Extract outgoing transitions
        /// </summary>
        /// <param name="references"></param>
        /// <param name="transitionId"></param>
        /// <returns></returns>
        private IList<KeyValuePair<string, string>> ExtractOutGoingTransitions(IEnumerable<XElement> references, string transitionId)
        {
            if (references == null) return null;
            var data = new List<KeyValuePair<string, string>>();
            foreach (var transitionRef in references)
            {
                var xRef = XSchema.Elements.FirstOrDefault(x => x.ID == transitionRef.Attributes.FirstOrDefault().Value);
                var targetRef = GetXElementFromTargetSequency(xRef);
                if (targetRef == null) continue;
                data.Add(new KeyValuePair<string, string>(transitionId, targetRef.ID));
            }
            return data;
        }

        /// <summary>
        /// Extract incoming 
        /// </summary>
        /// <param name="references"></param>
        /// <param name="transitionId"></param>
        /// <returns></returns>
        private IList<KeyValuePair<string, string>> ExtractIncomingTransitions(IEnumerable<XElement> references, string transitionId)
        {
            if (references == null) return null;
            var data = new List<KeyValuePair<string, string>>();
            foreach (var transitionRef in references)
            {
                var xRef = XSchema.Elements.FirstOrDefault(x => x.ID == transitionRef.Attributes.FirstOrDefault().Value);
                var targetRef = GetXElementFromSourceSequency(xRef);
                if (targetRef == null) continue;
                data.Add(new KeyValuePair<string, string>(transitionId, targetRef.ID));
            }
            return data;
        }
    }
}
