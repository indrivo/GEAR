using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;
using GR.Core.Abstractions;
using GR.Identity.Abstractions.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace GR.Identity.Abstractions
{
    [TrackEntity(Option = TrackEntityOption.SelectedFields)]
    public class GearUser : IdentityUser<Guid>, IBase<Guid>
    {
        /// <summary>
        /// Stores user first name
        /// </summary>
        [MaxLength(50)]
        [TrackField(Option = TrackFieldOption.Allow)]
        public virtual string FirstName { get; set; }

        /// <summary>
        /// Stores user last name
        /// </summary>
        [MaxLength(50)]
        [TrackField(Option = TrackFieldOption.Allow)]
        public virtual string LastName { get; set; }

        /// <summary>
        /// Is disabled field status
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public virtual bool IsDisabled { get; set; }

        /// <summary>
        /// Stores user birthday
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public virtual DateTime Birthday { get; set; }

        /// <summary>
        /// Stores same additional info about user
        /// </summary>
        [MaxLength(500)]
        public virtual string AboutMe { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Stores Id of the User that created the object
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public virtual string Author { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Stores the time when object was modified. Nullable
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public virtual DateTime Changed { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Stores the time when object was created
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public virtual DateTime Created { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Stores state of the Object. True if object is deleted and false otherwise
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public virtual bool IsDeleted { get; set; }

        /// <summary>
        /// Is editable field status
        /// </summary>
        public virtual bool IsEditable { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Stores the Id of the User that modified the object. Nullable
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public virtual string ModifiedBy { get; set; }

        /// <summary>
        /// Stores user photo
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public virtual byte[] UserPhoto { get; set; }

        /// <summary>
        /// Authentication Type
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public virtual AuthenticationType AuthenticationType { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Tenant id
        /// </summary>
        public virtual Guid? TenantId { get; set; }

        /// <summary>
        /// Last date password changed
        /// </summary>
        public virtual DateTime LastPasswordChanged { get; set; }

        /// <summary>
        /// Is password expired
        /// </summary>
        /// <returns></returns>
        public virtual bool IsPasswordExpired()
        {
            if (LastPasswordChanged == DateTime.MinValue) return false;
            var isExpired = (DateTime.Now - LastPasswordChanged).TotalDays >= 30;
            return isExpired;
        }

        /// <summary>
        /// Last login
        /// </summary>
        public virtual DateTime LastLogin { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Version
        /// </summary>
        public int Version { get; set; } = 1;

        /// <summary>
        /// Disable audit tracking
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public bool DisableAuditTracking { get; set; }
    }
}