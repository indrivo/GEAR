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

        /// <summary>
        /// Seed data async
        /// </summary>
        /// <returns></returns>
        public abstract Task SeedAsync();
    }
}
