using System;
using System.Collections.Generic;
using System.Text;
using GR.Documents.Abstractions.Models;

namespace GR.Documents.Abstractions.ViewModels.DocumentViewModels
{
    public class DocumentVersionViewModel
    {
        public virtual Guid Id { get; set; }
        public virtual Guid DocumentId { get; set; }
        public virtual string FileName { get; set; }
        public virtual string Url { get; set; }
        public virtual double VersionNumber { get; set; }
        public virtual bool IsArhive { get; set; }
        public virtual string Comments { get; set; }
        public virtual string CurrentStateName { get; set; }
        //public virtual Document Document { get; set; }
    }
}
