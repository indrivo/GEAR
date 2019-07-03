using System.Collections.Generic;
using ST.Identity.Abstractions;

namespace ST.MultiTenant.Razor.ViewModels
{
    public class CompanyUsersViewModel : ApplicationUser
    {
        public IEnumerable<string> Roles { get; set; }
    }
}
