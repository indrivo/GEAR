using GR.WorkFlows.Abstractions.Models;

namespace GR.WorkFlows.Abstractions.ViewModels
{
    public class EntryHistoryViewModel
    {
        public virtual string EntryId { get; set; }
        public virtual State FromState { get; set; }
        public virtual State ToState { get; set; }
        public virtual string Author { get; set; }
        public virtual string Message { get; set; }
    }
}
