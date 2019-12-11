using System.Collections.Generic;
using System.Diagnostics;
using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;
using GR.Core;

namespace GR.Identity.Abstractions
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