using System.Collections.Generic;
using System.Diagnostics;
using ST.Audit.Attributes;
using ST.Audit.Enums;
using ST.Entities.Models;

namespace ST.Identity.Data.Permissions
{
	/// <inheritdoc />
	/// <summary>
	/// Represents the authorization User group.
	/// </summary>
	[DebuggerDisplay(@"\{{" + nameof(Name) + @",nq}\}")]
	[TrackEntity(Option = TrackEntityOption.Selected)]
    public class AuthGroup : ExtendedModel
	{
        /// <summary>
        /// Name of the Group
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public string Name { get; set; }

		public List<UserGroup> UserGroups { get; set; }
	}
}