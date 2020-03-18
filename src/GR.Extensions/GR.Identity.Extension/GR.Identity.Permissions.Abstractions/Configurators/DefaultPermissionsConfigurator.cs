using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Identity.Permissions.Abstractions.Events.EventArgs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Identity.Permissions.Abstractions.Permissions;
using Activator = System.Activator;

namespace GR.Identity.Permissions.Abstractions.Configurators
{
    public class DefaultPermissionsConfigurator<TPermissionsConstants> : IPermissionsConfigurator
        where TPermissionsConstants : class
    {
        /// <summary>
        /// Inject context
        /// </summary>
        protected IPermissionsContext Context => IoC.Resolve<IPermissionsContext>();

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
        private IEnumerable<string> _permissions { get; set; } = new List<string>();

        /// <summary>
        /// Permissions
        /// </summary>
        public virtual IEnumerable<string> Permissions => _permissions.ToList();

        /// <summary>
        /// Inject identity context
        /// </summary>
        protected IIdentityContext IdentityContext => IoC.Resolve<IIdentityContext>();

        /// <summary>
        /// Trigger
        /// </summary>
        protected virtual void PermissionsSeedComplete()
        {
            OnPermissionsSeedComplete?.Invoke(this, new PermissionsSeedEventsArgs
            {
                Permissions = _permissions
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
            _permissions = permissions;
            return permissions;
        }

        /// <summary>
        /// Seed data async
        /// </summary>
        /// <returns></returns>
        public virtual async Task SeedAsync()
        {
            var permissions = GetModulePermissionsFromTargetModule();

            foreach (var permission in permissions)
            {
                var permissionConfig = permission.Split('_');
                await Context.Permissions.AddAsync(new Permission
                {
                    PermissionKey = permission,
                    PermissionName = permissionConfig[1],
                    ClientId = 1,
                    Description = $"Permission for module {permissionConfig[0]}"
                });
            }

            var dbResponse = await Context.PushAsync();
            if (dbResponse.IsSuccess)
                PermissionsSeedComplete();
            else
            {
                Console.WriteLine(dbResponse);
            }
        }
    }
}