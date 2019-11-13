using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using Activator = System.Activator;

namespace GR.Identity.Permissions.Abstractions.Configurators
{
    public abstract class DefaultPermissionsConfigurator<TPermissionsConstants> where TPermissionsConstants : class
    {
        public static IList<Func<IEnumerable<string>, Guid?, Task<bool>>> CustomRules;

        protected DefaultPermissionsConfigurator()
        {
            CustomRules.Add(async (permissions, userId) =>
            {
                if (permissions.Contains("InviteUser"))
                {
                    //check here
                    return true;
                }
                return false;
            });

            var haveAccess = CustomRules.Select(async rule => await rule(new List<string> { "permName" }, Guid.Empty))
                .Select(x => x.Result)
                .All(x => x);
        }

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

        public bool HasPermission(string permissionName)
        {
            return true;
        }
    }
}
