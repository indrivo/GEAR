using System.Collections.Generic;

namespace GR.Identity.Abstractions.ViewModels.RoleViewModels
{
    public class RolesViewModel : GearRole
    {
        public IEnumerable<string> Permissions { get; set; }
    }
}