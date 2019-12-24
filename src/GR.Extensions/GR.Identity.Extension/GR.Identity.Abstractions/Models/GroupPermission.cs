using GR.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Abstractions.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Group's permissions
    /// </summary>
    public class GroupPermission : BaseModel
    {
        /// <summary>
        /// The group this permission refers to
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// The Group object that will be populated when using
        /// the EF Include method. aka NavigationProperty
        /// </summary>
        public AuthGroup AuthGroup { get; set; }

        /// <summary>
        /// Represents a permission object represented as
        /// a formatted string with the following format:
        /// {Service:Module:Action}
        /// e.g.
        /// "Identity:Users:Create"
        /// </summary>
        [Required] public string PermissionCode { get; set; }
    }
}