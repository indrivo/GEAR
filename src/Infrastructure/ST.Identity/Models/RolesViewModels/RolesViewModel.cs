using System.Collections.Generic;
using ST.Identity.Data.Permissions;

namespace ST.Identity.Models.RolesViewModels
{
    public class RolesViewModel : ApplicationRole
    {
        public IEnumerable<string> Permissions { get; set; }
    }
}
