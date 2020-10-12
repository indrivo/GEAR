using System;
using GR.Core;
using GR.IdentityDocuments.Abstractions.Enums;

namespace GR.IdentityDocuments.Abstractions.Models
{
    public class IdentityDocument : BaseModel
    {
        /// <summary>
        /// UserKyc id
        /// </summary>
        public virtual UserKyc Kyc { get; set; }
        public virtual Guid UserKycId { get; set; }

        /// <summary>
        /// State
        /// </summary>
        public DocumentValidationState ValidationState { get; set; } = DocumentValidationState.Pending;

        /// <summary>
        /// Reject reason
        /// </summary>
        public virtual string Reason { get; set; }

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