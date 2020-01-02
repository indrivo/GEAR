using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Identity.Permissions.Abstractions.Events.EventArgs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Activator = System.Activator;

namespace GR.Identity.Permissions.Abstractions.Configurators
{
    public abstract class DefaultPermissionsConfigurator<TPermissionsConstants> where TPermissionsConstants : class
    {
        /// <summary>
        /// On permissions seed
        /// </summary>
        public event EventHandler<PermissionsSeedEventsArgs> OnPermissionsSeedComplete;

        protected DefaultPermissionsConfigurator()
        {
            OnPermissionsSeedComplete += (sender, args) =>
            {
                Debug.WriteLine("Permissions are seed");
            };
        }

        /// <summary>
        /// Module permissions
        /// </summary>
        private IEnumerable<string> Permissions { get; set; } = new List<string>();

        /// <summary>
        /// Inject identity context
        /// </summary>
        protected IIdentityContext IdentityContext => IoC.Resolve<IIdentityContext>();

        /// <summary>
        /// Trigger
        /// </summary>
        public void PermissionsSeedComplete()
        {
            OnPermissionsSeedComplete?.Invoke(this, new PermissionsSeedEventsArgs
            {
                Permissions = Permissions
            });
        }

        /// <summary>
        /// Get module permissions
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> GetModulePermissionsFromTargetModule()
        {
            var fieldInfo = typeof(TPermissionsConstants).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var o = Activator.CreateInstance<TPermissionsConstants>();
            var permissions = fieldInfo.Select(_ => _.GetValue(o).ToString()).ToList();
            Permissions = permissions;
            return permissions;
        }

        /// <summary>
        /// Seed data async
        /// </summary>
        /// <returns></returns>
        public abstract Task SeedAsync();
    }
}