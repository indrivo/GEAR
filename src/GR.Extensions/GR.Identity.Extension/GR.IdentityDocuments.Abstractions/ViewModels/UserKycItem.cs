using System;
using GR.IdentityDocuments.Abstractions.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GR.IdentityDocuments.Abstractions.ViewModels
{
    public class UserKycItem
    {
        /// <summary>
        /// Photo url
        /// </summary>
        public virtual string PhotoUrl => $"/Users/GetImage?id={UserId}";

        /// <summary>
        /// Email
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Full name
        /// </summary>
        public virtual string FullName { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        public virtual string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public virtual string LastName { get; set; }

        /// <summary>
        /// Phone number
        /// </summary>
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        public virtual Guid UserId { get; set; }

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
    }
}
