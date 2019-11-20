using System;
using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.Documents.Abstractions.Models
{
    public class DocumentVersion: BaseModel
    {

        /// <summary>
        /// Document Id
        /// </summary>
        [Required]
        public virtual Guid DocumentId { get; set; }

        /// <summary>
        /// Document
        /// </summary>
        public virtual Document Document { get; set; }


        /// <summary>
        /// File Id
        /// </summary>
        public virtual Guid? FileStorageId { get; set; }


        ///// <summary>
        ///// File
        ///// </summary>
         public virtual string FileName { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        public virtual string Url { get; set; }


        /// <summary>
        /// Version number
        /// </summary>
        [Required]
        public virtual double VersionNumber { get; set; }

        /// <summary>
        /// Is arhiv
        /// </summary>
        [Required]
        public virtual bool IsArhive { get; set; }


        /// <summary>
        /// Comments
        /// </summary>
        public virtual string Comments { get; set; }

        /// <summary>
        ///  Previous version id
        /// </summary>
        public virtual Guid? PreviousVersionId { get; set; }

        /// <summary>
        /// Previous version
        /// </summary>
        public virtual DocumentVersion PreviousVersion { get; set; }


        /// <summary>
        /// Major version
        /// </summary>
        [Required]
        public virtual bool IsMajorVersion { get; set; }

        /// <summary>
        /// Owner id
        /// </summary>
         [Required]
        public virtual Guid OwnerId { get; set; }

    }
}
