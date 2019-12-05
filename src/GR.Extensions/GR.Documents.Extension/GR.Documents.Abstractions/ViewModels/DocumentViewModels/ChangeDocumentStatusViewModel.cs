using System;
using System.Collections.Generic;
using System.Text;

namespace GR.Documents.Abstractions.ViewModels.DocumentViewModels
{
    public class ChangeDocumentStatusViewModel
    {
        public string EntryId { get; set; }
        public Guid? WorkFlowId { get; set; }
        public Guid? NewStateId { get; set; }
    }
}
