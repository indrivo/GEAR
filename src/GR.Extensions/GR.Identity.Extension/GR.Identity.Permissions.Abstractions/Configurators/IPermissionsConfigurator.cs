using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Identity.Permissions.Abstractions.Events.EventArgs;

namespace GR.Identity.Permissions.Abstractions.Configurators
{
    public interface IPermissionsConfigurator
    {
        event EventHandler<PermissionsSeedEventsArgs> OnPermissionsSeedComplete;
        Dictionary<string, string> Permissions { get; }
        Dictionary<string, string> GetModulePermissionsFromTargetModule();
        Task SeedAsync();
    }
}