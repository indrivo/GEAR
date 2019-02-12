using System;
using System.Collections.Generic;
using ST.Audit.Attributes;
using ST.Audit.Enums;
using ST.Identity.Data.Permissions;
using ST.Identity.LDAP.Models;

namespace ST.Identity.Data.UserProfiles
{
    [TrackEntity(Option = TrackEntityOption.Selected)]
    public class ApplicationUser : LdapUser
    {
        /// <summary>
        /// Stores Id of the User that created the object
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public string Author { get; set; }
        /// <summary>
        /// Stores the time when object was modified. Nullable
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public DateTime Changed { get; set; }

        /// <summary>
        /// Stores the time when object was created
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public DateTime Created { get; set; }

        /// <summary>
        /// Stores state of the Object. True if object is deleted and false otherwise
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Is editable field status
        /// </summary>
        public bool IsEditable { get; set; }

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
    }
}