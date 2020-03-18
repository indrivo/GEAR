using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Identity.Permissions.Abstractions.Events.EventArgs;

namespace GR.Identity.Permissions.Abstractions.Configurators
{
    public interface IPermissionsConfigurator
    {
        event EventHandler<PermissionsSeedEventsArgs> OnPermissionsSeedComplete;
        IEnumerable<string> Permissions { get; }
        IEnumerable<string> GetModulePermissionsFromTargetModule();
        Task SeedAsync();
    }
}