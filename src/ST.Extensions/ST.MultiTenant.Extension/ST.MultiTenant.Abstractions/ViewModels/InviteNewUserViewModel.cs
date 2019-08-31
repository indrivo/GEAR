using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ST.MultiTenant.Abstractions.ViewModels
{
    public class InviteNewUserViewModel
    {
        public InviteNewUserViewModel()
        {
            Roles = new HashSet<string>();
        }

        [Required]
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}