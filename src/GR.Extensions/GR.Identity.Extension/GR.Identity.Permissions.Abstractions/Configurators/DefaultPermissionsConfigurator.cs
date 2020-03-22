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
using Microsoft.EntityFrameworkCore;
using Activator = System.Activator;

namespace GR.Identity.Permissions.Abstractions.Configurators
{
    public class DefaultPermissionsConfigurator<TPermissionsConstants> : IPermissionsConfigurator
        where TPermissionsConstants : class
    {
        /// <summary>
        /// Module permissions
        /// </summary>
        private readonly Dictionary<string, string> _permissions;

        /// <summary>
        /// Inject context
        /// </summary>
        protected IPermissionsContext Context => IoC.Resolve<IPermissionsContext>();

        /// <summary>
        /// On permissions seed
        /// </summary>
        public event EventHandler<PermissionsSeedEventsArgs> OnPermissionsSeedComplete;

        public DefaultPermissionsConfigurator()
        {
            OnPermissionsSeedComplete += (sender, args) =>
            {
                Debug.WriteLine("Permissions are seed");
            };

            _permissions = GetModulePermissionsFromTargetModule();
        }

        /// <summary>
        /// Permissions
        /// </summary>
        public virtual Dictionary<string, string> Permissions => _permissions;

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
        public Dictionary<string, string> GetModulePermissionsFromTargetModule()
        {
            var fieldInfo = typeof(TPermissionsConstants).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var o = Activator.CreateInstance<TPermissionsConstants>();
            var permissions = fieldInfo
                .ToDictionary(k => k.Name, v => v.GetValue(o).ToString());
            return permissions;
        }

        /// <summary>
        /// Seed data async
        /// </summary>
        /// <returns></returns>
        public virtual async Task SeedAsync()
        {
            foreach (var permission in Permissions)
            {
                if (await Context.Permissions.AnyAsync(x => x.PermissionKey == permission.Key)) continue;
                await Context.Permissions.AddAsync(new Permission
                {
                    PermissionKey = permission.Key,
                    PermissionName = permission.Value,
                    ClientId = 1,
                    Description = $"Permission for module {permission.Key}"
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