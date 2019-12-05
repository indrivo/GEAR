using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GR.Documents.Abstractions.Models;
using GR.WorkFlows.Abstractions.Models;
using GR.WorkFlows.Abstractions.ViewModels;
using Microsoft.AspNetCore.Http;

namespace GR.Documents.Abstractions.ViewModels.DocumentViewModels
{
    public class DocumentViewModel
    {
        public Guid Id { get; set; }
        
        public virtual string Author { get; set; }
        
        public DateTime Created { get; set; }
        
        public virtual string ModifiedBy { get; set; }

        public DateTime Changed { get; set; }

        public virtual bool IsDeleted { get; set; }
       
        public virtual int Version { get; set; }
       
        public virtual Guid? TenantId { get; set; }
        
        public virtual string DocumentCode { get; set; }
       
       
        public virtual string Title { get; set; }
        
        public virtual string Description { get; set; }
        
        public virtual string Group { get; set; }        
       
        public virtual Guid DocumentTypeId { get; set; }
       
        public virtual DocumentType DocumentType { get; set; }
       
       // public virtual List<DocumentVersion> DocumentVersions { get; set; }
       
        public virtual Guid UserId { get; set; }
        
        public virtual Guid? LastFileId { get; set; }
       
        public virtual string FileName { get; set; }
       
        public virtual string LastUrl { get; set; }
       
        public virtual double LastVersion { get; set; }

        public virtual Guid? LastVersionId { get; set; }

        public string CurrentStateName { get; set; }

        public List<StateGetViewModel> ListNextState { get; set; }

    }
}
