using GR.Identity.Abstractions;
using System.Collections.Generic;

namespace GR.Identity.Models.RolesViewModels
{
    public class RolesViewModel : GearRole
    {
        public IEnumerable<string> Permissions { get; set; }
    }
}