using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Core;
using GR.Identity.Abstractions;
using GR.IdentityDocuments.Abstractions.Enums;

namespace GR.IdentityDocuments.Abstractions.Models
{
    public class UserKyc : BaseModel
    {
        /// <summary>
        /// User id
        /// </summary>
        public virtual GearUser User { get; set; }
        [Required]
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// State
        /// </summary>
        public DocumentValidationState ValidationState { get; set; } = DocumentValidationState.Pending;

        /// <summary>
        /// Reject reason
        /// </summary>
        public virtual string Reason { get; set; }

        /// <summary>
        /// Documents
        /// </summary>
        public ICollection<IdentityDocument> Documents { get; set; } = new List<IdentityDocument>();
    }
}