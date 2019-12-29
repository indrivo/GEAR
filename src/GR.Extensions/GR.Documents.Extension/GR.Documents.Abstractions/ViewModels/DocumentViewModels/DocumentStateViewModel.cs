using System;
using System.Collections.Generic;
using System.Text;
using GR.WorkFlows.Abstractions.ViewModels;

namespace GR.Documents.Abstractions.ViewModels.DocumentViewModels
{
    public class DocumentStateViewModel
    {
        public virtual Guid Id { get; set; }

        public virtual string CurrentStateName { get; set; }

        public virtual IEnumerable<StateGetViewModel> ListNextState { get; set; }
        
        public virtual IEnumerable<EntryHistoryViewModel> History { get; set; }

    }
}
