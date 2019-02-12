using System;
using Microsoft.AspNetCore.Identity;
using ST.Audit.Attributes;
using ST.Audit.Enums;

namespace ST.Identity.Data.Permissions
{
    /// <inheritdoc />
    /// <summary>
    /// Represents the Role of the user
    /// </summary>
    [TrackEntity(Option = TrackEntityOption.Selected)]
    public class ApplicationRole : IdentityRole<string>
	{
		/// <inheritdoc />
		/// <summary>
		/// Constructs a new <see cref="T:ST.Identity.Data.Permissions.ApplicationRole" /> object that
		/// has no name
		/// </summary>
		/// <returns></returns>
		public ApplicationRole() => Id = Guid.NewGuid().ToString();

		/// <inheritdoc />
		/// <summary>
		/// Constructs a new ApplicationRole object with the
		/// specified name
		/// </summary>
		/// <param name="roleName">Name of the role e.g. Admin</param>
		/// <returns>A new ApplicationRole Instance</returns>
		public ApplicationRole(string roleName) : this() => Name = roleName;

		/// <summary>
		/// Implicit conversion operator to convert a string object
		/// that represents the Role Name, to an actual ApplicationRole Object
		/// </summary>
		/// <param name="roleName">Name of the role</param>
		public static implicit operator ApplicationRole(string roleName) => new ApplicationRole
		{
			Name = roleName
		};
        /// <summary>
        /// Stores Id of the User that created the object
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Stores the time when object was created
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public DateTime Created { get; set; }
        /// <summary>
        /// Stores the Id of the User that modified the object. Nullable
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public string ModifiedBy { get; set; }
        /// <summary>
        /// Stores the time when object was modified. Nullable
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public DateTime Changed { get; set; }
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
	}
}