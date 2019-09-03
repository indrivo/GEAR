using System.Collections.Generic;
using System.Diagnostics;
using ST.Audit.Abstractions.Attributes;
using ST.Audit.Abstractions.Enums;
using ST.Core;

namespace ST.Identity.Abstractions
{
	/// <inheritdoc />
	/// <summary>
	/// Represents the authorization User group.
	/// </summary>
	[DebuggerDisplay(@"\{{" + nameof(Name) + @",nq}\}")]
	[TrackEntity(Option = TrackEntityOption.SelectedFields)]
    public class AuthGroup : BaseModel
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