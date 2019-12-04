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

        /// <summary>Stores Id of the Object</summary>
        public Guid Id { get; set; }

        /// <inheritdoc />
        /// <summary>Stores Id of the User that created the object</summary>
        public virtual string Author { get; set; }

        /// <inheritdoc />
        /// <summary>Stores the time when object was created</summary>
        public DateTime Created { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Stores the Id of the User that modified the object. Nullable
        /// </summary>
        public virtual string ModifiedBy { get; set; }

        /// <inheritdoc />
        /// <summary>Stores the time when object was modified. Nullable</summary>
        public DateTime Changed { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Stores state of the Object. True if object is deleted and false otherwise
        /// </summary>
        public virtual bool IsDeleted { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Version of data
        /// </summary>
        public virtual int Version { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Tenant id
        /// </summary>
        public virtual Guid? TenantId { get; set; }
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
        public virtual Guid? LastFileId { get; set; }


        /// <summary>
        /// File name 
        /// </summary>
        public virtual string FileName { get; set; }

        /// <summary>
        /// File url 
        /// </summary>
        public virtual string LastUrl { get; set; }

        /// <summary>
        /// File version
        /// </summary>
        public virtual double LastVersion { get; set; }

        //public EntryState CurentState { get; set; }
        //public IEnumerable<StateGetViewModel> LiestNextState { get; set; }

    }
}
