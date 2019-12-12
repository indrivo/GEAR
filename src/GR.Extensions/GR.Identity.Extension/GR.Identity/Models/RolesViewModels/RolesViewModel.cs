using System.Collections.Generic;
using GR.Identity.Abstractions;
using GR.Identity.Data.Permissions;

namespace GR.Identity.Models.RolesViewModels
{
    public class RolesViewModel : GearRole
    {
        public IEnumerable<string> Permissions { get; set; }
    }
}
