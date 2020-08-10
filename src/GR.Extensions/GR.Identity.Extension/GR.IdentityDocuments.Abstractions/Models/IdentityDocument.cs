using System;
using GR.Core;
using GR.Identity.Abstractions;

namespace GR.IdentityDocuments.Abstractions.Models
{
    public class IdentityDocument : BaseModel
    {
        /// <summary>
        /// User id
        /// </summary>
        public virtual GearUser User { get; set; }
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// Blob
        /// </summary>
        public virtual byte[] Blob { get; set; }

        /// <summary>
        /// Document type
        /// </summary>
        public virtual string DocumentType { get; set; }

        /// <summary>
        /// Full name
        /// </summary>
        public virtual string FileName { get; set; }

        /// <summary>
        /// Content type
        /// </summary>
        public virtual string ContentType { get; set; }
    }
}