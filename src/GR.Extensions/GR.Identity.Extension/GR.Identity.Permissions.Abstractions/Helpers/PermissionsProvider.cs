using System.Collections.Concurrent;
using System.Threading.Tasks;
using GR.Identity.Permissions.Abstractions.Configurators;

namespace GR.Identity.Permissions.Abstractions.Helpers
{
    public static class PermissionsProvider
    {
        /// <summary>
        /// Configurators
        /// </summary>
        internal static ConcurrentQueue<IPermissionsConfigurator> Configurators = new ConcurrentQueue<IPermissionsConfigurator>();

        /// <summary>
        /// Invoke seed modules permissions
        /// </summary>
        /// <returns></returns>
        public static async Task InvokeAsync()
        {
            while (Configurators.TryDequeue(out var configurator))
            {
                await configurator.SeedAsync();
            }
        }
    }
}