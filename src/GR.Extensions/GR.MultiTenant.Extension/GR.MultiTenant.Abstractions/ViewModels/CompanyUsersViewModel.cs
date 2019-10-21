using System.Collections.Generic;
using GR.Identity.Abstractions;

namespace GR.MultiTenant.Abstractions.ViewModels
{
    public class CompanyUsersViewModel : ApplicationUser
    {
        public IEnumerable<string> Roles { get; set; }
        public bool IsOnline { get; set; }
    }
}
