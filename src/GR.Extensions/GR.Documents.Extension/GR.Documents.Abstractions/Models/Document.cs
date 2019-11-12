using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using GR.Core;
using GR.Subscriptions.Abstractions.Models;

namespace GR.Documents.Abstractions.Models
{
    public class Document: BaseModel
    {
        /// <summary>
        /// Document Code
        /// </summary>
        public string DocumentCode { get; set; }

        /// <summary>
        /// Title
        /// </summary>
         [Required]
        public string Title { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Group
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Document Type id
        /// </summary>
        [Required]
        public Guid DodumentTypeId { get; set; }


        /// <summary>
        /// Document type
        /// </summary>
        public DocumentType DocumentType { get; set; }

        /// <summary>
        /// List document versions
        /// </summary>
        public IEnumerable<DocumentVersion> DocumentVersions { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        [Required]
        public Guid UserId { get; set; }
    }
}
