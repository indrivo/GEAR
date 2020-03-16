using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;
using GR.Core.Abstractions;
using GR.Identity.Abstractions.Enums;
using GR.Identity.Abstractions.Models.AddressModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GR.Identity.Abstractions
{
    [TrackEntity(Option = TrackEntityOption.SelectedFields)]
    public class GearUser : IdentityUser<Guid>, IBaseModel
    {
        /// <summary>
        /// Stores user first name
        /// </summary>
        [MaxLength(50)]
        [TrackField(Option = TrackFieldOption.Allow)]
        public string UserFirstName { get; set; }

        /// <summary>
        /// Stores user last name
        /// </summary>
        [MaxLength(50)]
        [TrackField(Option = TrackFieldOption.Allow)]
        public string UserLastName { get; set; }

        /// <summary>
        /// Is disabled field status
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Stores user birthday
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public DateTime Birthday { get; set; }

        /// <summary>
        /// Stores same additional info about user
        /// </summary>
        [MaxLength(500)]
        public string AboutMe { get; set; }

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

        /// <summary>
        /// User addresses
        /// </summary>
        public ICollection<Address> Addresses { get; set; }

        [NotMapped]
        [Display(Name = "Password")]
        [Required(ErrorMessage = "You must enter your password!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}