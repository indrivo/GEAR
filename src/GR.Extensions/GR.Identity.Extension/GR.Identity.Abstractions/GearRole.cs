using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;
using GR.Core.Abstractions;
using Microsoft.AspNetCore.Identity;
using System;

namespace GR.Identity.Abstractions
{
    /// <summary>
    /// Represents the Role of the user
    /// </summary>
    [TrackEntity(Option = TrackEntityOption.SelectedFields)]
    public class GearRole : IdentityRole<string>, IBaseModel
    {
        /// <inheritdoc />
        /// <summary>
        /// Constructs a new <see cref="T:GR.Identity.Abstractions.ApplicationRole" /> object that
        /// has no name
        /// </summary>
        /// <returns></returns>
        public GearRole() => Id = Guid.NewGuid().ToString();

        /// <inheritdoc />
        /// <summary>
        /// Constructs a new ApplicationRole object with the
        /// specified name
        /// </summary>
        /// <param name="roleName">Name of the role e.g. Admin</param>
        /// <returns>A new ApplicationRole Instance</returns>
        public GearRole(string roleName) : this() => Name = roleName;

        /// <summary>
        /// Implicit conversion operator to convert a string object
        /// that represents the Role Name, to an actual ApplicationRole Object
        /// </summary>
        /// <param name="roleName">Name of the role</param>
        public static implicit operator GearRole(string roleName) => new GearRole
        {
            Name = roleName
        };

        /// <inheritdoc />
        /// <summary>
        /// Stores Id of the User that created the object
        /// </summary>
        public string Author { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Stores the time when object was created
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public DateTime Created { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Stores the Id of the User that modified the object. Nullable
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public string ModifiedBy { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Stores the time when object was modified. Nullable
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public DateTime Changed { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///  Stores state of the Object. True if object is deleted and false otherwise
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Client Id
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public int ClientId { get; set; }

        /// <summary>
        /// Stores Id of the User that created the object
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public string Title { get; set; }

        /// <summary>
        /// Is editable or no
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public bool IsNoEditable { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Name
        /// </summary>
	    [TrackField(Option = TrackFieldOption.Allow)]
        public override string Name { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Tenant id
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Version
        /// </summary>
        public int Version { get; set; }
    }
}