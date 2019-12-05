using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using GR.Core;

namespace GR.Documents.Abstractions.Models
{
    public class Document: BaseModel
    {
        /// <summary>
        /// Document Code
        /// </summary>
        public virtual string DocumentCode { get; set; }

        /// <summary>
        /// Title
        /// </summary>
         [Required]
        public virtual string Title { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Group
        /// </summary>
        public virtual string Group { get; set; }

        /// <summary>
        /// Document Type id
        /// </summary>
        [Required]
        public virtual Guid DocumentTypeId { get; set; }


        /// <summary>
        /// Document type
        /// </summary>
        public virtual DocumentType DocumentType { get; set; }

        /// <summary>
        /// List document versions
        /// </summary>
        public virtual IEnumerable<DocumentVersion> DocumentVersions { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        [Required]
        public virtual Guid UserId { get; set; }


        /// <summary>
        /// last File Id
        /// </summary>
        public virtual Guid? LastFileId => DocumentVersions.ToList()?.OrderBy(x => x.VersionNumber).LastOrDefault()?.FileStorageId;

        /// <summary>
        /// last version Id
        /// </summary>
        public virtual Guid? LastVersionId =>DocumentVersions.ToList()?.OrderBy(x => x.VersionNumber).LastOrDefault()?.Id;

        /// <summary>
        /// File name 
        /// </summary>
        public virtual string FileName => DocumentVersions.ToList()?.OrderBy(x => x.VersionNumber).LastOrDefault()?.FileName;

        /// <summary>
        /// File url 
        /// </summary>
        public virtual string LastUrl => DocumentVersions.ToList()?.OrderBy(x => x.VersionNumber).LastOrDefault()?.Url;

        /// <summary>
        /// File version
        /// </summary>
        public virtual double LastVersion =>  DocumentVersions.ToList()?.OrderBy(x => x.VersionNumber).LastOrDefault()?.VersionNumber ?? 1;
    }
}
