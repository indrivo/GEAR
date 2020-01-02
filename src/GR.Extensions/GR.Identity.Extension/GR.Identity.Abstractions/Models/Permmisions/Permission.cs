using GR.Core;
using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Abstractions.Models.Permmisions
{
    public class Permission : BaseModel
    {
        /// <summary>
        /// Permission name
        /// </summary>
        [MaxLength(100)]
        public string PermissionName { get; set; }

        /// <summary>
        /// Store client id
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Set a description for permissions
        /// </summary>
        [MaxLength(300)]
        public string Description { get; set; }

        /// <summary>
        /// Unique permission name used by system
        /// </summary>
        [MaxLength(100)]
        public string PermissionKey { get; set; }
    }
}