using System.Collections.Generic;
using System.Linq;

namespace GR.Procesess.Models.ParserModels
{
    public class XSchema
    {
        public string ID;
        public string Name;
        public List<XElement> Elements;
        public XElement Root;

        public bool IsCollaborationDiagram()
            => this.Elements.Any(x => x.TypeName == XElementType.Collaboration.ToString());


        public IEnumerable<XElement> GetCollaborations()
            => this.Elements?.Where(x => x.TypeName == XElementType.Collaboration.ToString()).ToList();


        public IEnumerable<XElement> GetParticipantsbyCollaborationId(string collaborationId)
        {
            return this.Elements
                     .Where(x => x.TypeName == XElementType.Participant.ToString()
                     && x.ParentID == collaborationId).ToList();
        }

        public XElement GetProcessFromParticipant(XElement xParticipant)
        {
            return this.Elements.FirstOrDefault(x => x.ID == xParticipant.Attributes["processRef"]);
        }

        public IEnumerable<XElement> GetActorsFromProcess(XElement xProcess)
        {
            return this.Elements.Where(x => x.TypeName == XElementType.Lane.ToString()
                                                && x.ParentID == xProcess.ID);
        }
    }

    public class XElement
    {
        public string ParentID;
        public string TypeName;
        public Dictionary<string, string> Attributes;
        public Dictionary<string, List<string>> Properties;
        public Dictionary<string, List<XElement>> Elements;

        public string ID { get; set; }
    }
}
