using System;
using System.Collections.Generic;
using ST.Audit.Attributes;
using ST.Audit.Enums;
using ST.Core.Abstractions;
using ST.Identity.Abstractions.Enums;
using ST.Identity.LdapAuth.Abstractions.Models;

namespace ST.Identity.Abstractions
{
    [TrackEntity(Option = TrackEntityOption.SelectedFields)]
    public class ApplicationUser : LdapUser, IBaseModel
    {
        /// <inheritdoc />
        /// <summary>
        /// Stores Id of the User that created the object
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public string Author { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// Stores the time when object was modified. Nullable
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public DateTime Changed { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Stores the time when object was created
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public DateTime Created { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Stores state of the Object. True if object is deleted and false otherwise
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Is editable field status
        /// </summary>
        public bool IsEditable { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Stores the Id of the User that modified the object. Nullable
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public string ModifiedBy { get; set; }

        /// <summary>
        /// Add to user group
        /// </summary>
        public List<UserGroup> UserGroups { get; set; }

        /// <summary>
        /// Stores user photo
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public byte[] UserPhoto { get; set; }
        /// <summary>
        /// Authentication Type
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public AuthenticationType AuthenticationType { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Tenant id
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Last date password changed
        /// </summary>
        public DateTime LastPasswordChanged { get; set; }

        /// <summary>
        /// Is password expired
        /// </summary>
        /// <returns></returns>
        public bool IsPasswordExpired()
        {
            if (LastPasswordChanged == DateTime.MinValue) return false;
            var isExpired = (DateTime.Now - LastPasswordChanged).TotalDays >= 30;
            return isExpired;
        }

        /// <summary>
        /// Last login
        /// </summary>
        public DateTime LastLogin { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Version
        /// </summary>
        public int Version { get; set; } = 1;
    }
}