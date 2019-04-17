using System.Collections.Generic;
using System.Diagnostics;
using ST.Audit.Attributes;
using ST.Audit.Enums;
using ST.Shared;

namespace ST.Identity.Data.Permissions
{
	/// <inheritdoc />
	/// <summary>
	/// Represents the authorization User group.
	/// </summary>
	[DebuggerDisplay(@"\{{" + nameof(Name) + @",nq}\}")]
	[TrackEntity(Option = TrackEntityOption.SelectedFields)]
    public class AuthGroup : ExtendedModel
	{
        /// <summary>
        /// Name of the Group
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public string Name { get; set; }

        /// <summary>
        /// User groups
        /// </summary>
		public List<UserGroup> UserGroups { get; set; }
    }
}