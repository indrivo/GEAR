using System;

namespace GR.WorkFlows.Abstractions.ViewModels
{
    public class StateGetViewModel : AddNewStateViewModel
    {
        public virtual Guid Id { get; set; }

        public virtual bool IsStartState { get; set; }
        public virtual bool IsEndState { get; set; }
    }
}
