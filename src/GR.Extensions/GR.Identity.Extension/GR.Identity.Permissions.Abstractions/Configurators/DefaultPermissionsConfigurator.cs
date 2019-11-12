using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Identity.Permissions.Abstractions.Events.EventArgs;
using Activator = System.Activator;

namespace GR.Identity.Permissions.Abstractions.Configurators
{
    public abstract class DefaultPermissionsConfigurator<TPermissionsConstants> where TPermissionsConstants : class
    {
        /// <summary>
        /// Executed handlers
        /// </summary>
        private long _executedHandlers = 0;

        protected DefaultPermissionsConfigurator()
        {
            AttachHandler((sender, args) => { ++_executedHandlers; });
        }

        /// <summary>
        /// On permission request
        /// </summary>
        public virtual event EventHandler<PermissionRequestEventArgs> OnPermissionRequest;

        /// <summary>
        /// Inject identity context
        /// </summary>
        protected IIdentityContext IdentityContext => IoC.Resolve<IIdentityContext>();

        /// <summary>
        /// Get module permissions
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> GetModulePermissionsFromTargetModule()
        {
            var fieldInfo = typeof(TPermissionsConstants).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var o = Activator.CreateInstance<TPermissionsConstants>();
            var permissions = fieldInfo.Select(_ => _.GetValue(o).ToString()).ToList();
            return permissions;
        }

        public void AttachHandler(EventHandler<PermissionRequestEventArgs> handler)
        {
            OnPermissionRequest += handler;
        }

        public bool HasPermission(string permissionName)
        {
            OnPermissionRequest?.Invoke(this, new PermissionRequestEventArgs
            {
                PermissionName = permissionName
            });

            var handlersCount = OnPermissionRequest?.GetInvocationList().Length ?? 0;
            while (handlersCount != _executedHandlers) { }

            return true;
        }
    }
}
