using System;
using System.Collections.Generic;
using GR.IdentityDocuments.Abstractions.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GR.IdentityDocuments.Abstractions.ViewModels
{
    public class UserKycInfo
    {
        /// <summary>
        /// User kyc
        /// </summary>
        public virtual Guid KycId { get; set; }

        /// <summary>
        /// Submit date
        /// </summary>
        public virtual DateTime SubmitDate { get; set; }

        /// <summary>
        /// Pending documents for verification
        /// </summary>
        public virtual int PendingDocuments { get; set; }

        /// <summary>
        /// State
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DocumentValidationState ValidationState { get; set; } = DocumentValidationState.Pending;

        /// <summary>
        /// Reason
        /// </summary>
        public virtual string Reason { get; set; }

        /// <summary>
        /// Documents
        /// </summary>
        public virtual ICollection<UserKycDocumentInfo> Documents { get; set; } = new List<UserKycDocumentInfo>();
    }

    public class UserKycDocumentInfo
    {
        /// <summary>
        /// Data url
        /// </summary>
        public virtual string DataUrl => $"/api/IdentityDocumentsApi/GetDocumentImage?id={DocumentId}";

        /// <summary>
        /// Id
        /// </summary>
        public virtual Guid DocumentId { get; set; }

        /// <summary>
        /// State
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DocumentValidationState ValidationState { get; set; } = DocumentValidationState.Pending;

        /// <summary>
        /// Reason
        /// </summary>
        public virtual string Reason { get; set; }

        /// <summary>
        /// Document type
        /// </summary>
        public virtual IDocumentType DocumentType { get; set; }
    }
}