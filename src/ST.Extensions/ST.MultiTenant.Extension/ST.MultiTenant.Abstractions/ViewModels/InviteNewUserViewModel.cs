using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ST.MultiTenant.Abstractions.ViewModels
{
    public class InviteNewUserViewModel
    {
        /// <summary>
        /// Email
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// User roles
        /// </summary>
        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}