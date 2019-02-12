using System.ComponentModel.DataAnnotations;
using ST.Entities.Models;

namespace ST.Identity.Data.Permissions
{
    public class Permission : ExtendedModel
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
