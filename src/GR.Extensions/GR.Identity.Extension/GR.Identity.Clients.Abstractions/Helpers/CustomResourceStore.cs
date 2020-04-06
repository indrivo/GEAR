using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Mappers;

namespace GR.Identity.Clients.Abstractions.Helpers
{
    public class CustomResourceStore : IResourceStore
    {
        #region Injectable

        /// <summary>
        /// Inject clients context
        /// </summary>
        private readonly IClientsService _clientsService;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientsService"></param>
        public CustomResourceStore(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        /// <summary>
        /// Find api resource
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual async Task<ApiResource> FindApiResourceAsync(string name)
        {
            var apiResources = await _clientsService.GetApiResourcesAsync();
            var apiResource = apiResources?.FirstOrDefault(x => x.Name == name)?.ToModel();
            return apiResource;
        }

        /// <summary>
        /// Find api resource by scope
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var apiResources = (await _clientsService.GetApiResourcesAsync())?
                .Select(x => x.ToModel());
            return apiResources?.Select(a => new
            {
                a,
                scopes = a.Scopes.Where(s => scopeNames.Contains(s.Name))
            }).Where(x => x.scopes.Any()).Select(y => y.a);
        }

        /// <summary>
        /// Find identity resources by scope
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var identityResources = (await _clientsService.GetIdentityResourcesAsync())?.Select(x => x.ToModel());
            return identityResources?.Where(
                i => scopeNames.Contains(i.Name));
        }

        /// <summary>
        /// Get all resources
        /// </summary>
        /// <returns></returns>
        public virtual async Task<Resources> GetAllResourcesAsync()
        {
            var identityResources = (await _clientsService.GetIdentityResourcesAsync())?.Select(x => x.ToModel());
            var apiResources = (await _clientsService.GetApiResourcesAsync())?.Select(x => x.ToModel());
            return new Resources(identityResources, apiResources);
        }
    }
}
