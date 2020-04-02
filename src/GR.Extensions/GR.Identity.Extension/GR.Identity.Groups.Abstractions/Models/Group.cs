using System.Collections.Generic;
using System.Diagnostics;
using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;
using GR.Core;

namespace GR.Identity.Groups.Abstractions.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Represents the authorization User group.
    /// </summary>
    [DebuggerDisplay(@"\{{" + nameof(Name) + @",nq}\}")]
    [TrackEntity(Option = TrackEntityOption.SelectedFields)]
    public class Group : BaseModel
    {
        public Group() { }

        public Group(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Name of the Group
        /// </summary>
        [TrackField(Option = TrackFieldOption.Allow)]
        public string Name { get; set; }

        /// <summary>
        /// User groups
        /// </summary>
		public ICollection<UserGroup> UserGroups { get; set; }
    }
}