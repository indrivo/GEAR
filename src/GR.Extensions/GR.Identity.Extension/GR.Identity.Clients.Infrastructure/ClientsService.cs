using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Cache.Abstractions;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Identity.Clients.Abstractions;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Clients.Infrastructure
{
    public class ClientsService : IClientsService
    {
        private const string ClientsCacheKey = "Gear_Clients_";
        private const string ApiResourceCacheKey = "Gear_Api_Resources_";
        private const string IdentityResourceCacheKey = "Gear_Identity_Resources_";

        #region Injectable

        /// <summary>
        /// Inject clients context
        /// </summary>
        private readonly IClientsContext _clientsContext;

        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly ICacheService _cacheService;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientsContext"></param>
        /// <param name="cacheService"></param>
        public ClientsService(IClientsContext clientsContext, ICacheService cacheService)
        {
            _clientsContext = clientsContext;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Get app name
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public virtual async Task<string> GetAppName(string appId)
        {
            var appName = await _clientsContext.Clients.FirstOrDefaultAsync(x => x.ClientId.Equals("core"));
            return appName?.ClientName ?? "No app found";
        }

        /// <summary>
        /// Get all clients
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<Client>> GetAllClientsAsync(bool resetCache = false)
        {
            if (!resetCache)
            {
                var cacheClients = await _cacheService.GetAsync<IEnumerable<Client>>(ClientsCacheKey);
                if (cacheClients != null) return cacheClients.ToList();
            }

            var clients = await _clientsContext.Clients
                .Include(x => x.AllowedCorsOrigins)
                .Include(x => x.AllowedGrantTypes)
                .Include(x => x.AllowedScopes)
                .Include(x => x.Claims)
                .Include(x => x.ClientSecrets)
                .Include(x => x.IdentityProviderRestrictions)
                .Include(x => x.RedirectUris)
                .Include(x => x.Properties)
                .Include(x => x.PostLogoutRedirectUris)
                .ToListAsync();
            foreach (var client in clients)
            {
                client.AllowedGrantTypes = client.AllowedGrantTypes.DistinctBy(x => x.GrantType).ToList();
                client.AllowedScopes = client.AllowedScopes.DistinctBy(x => x.Scope).ToList();
            }
            await _cacheService.SetAsync(ClientsCacheKey, clients);
            return clients;
        }

        /// <summary>
        /// Get api resources
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<ApiResource>> GetApiResourcesAsync()
        {
            var cacheApiResources = await _cacheService.GetAsync<IEnumerable<ApiResource>>(ApiResourceCacheKey);
            if (cacheApiResources != null) return cacheApiResources.ToList();
            var apiResources = await _clientsContext.ApiResources
                .Include(x => x.Properties)
                .Include(x => x.Scopes)
                .Include(x => x.Secrets)
                .Include(x => x.UserClaims)
                .ToListAsync();
            await _cacheService.SetAsync(ApiResourceCacheKey, apiResources);
            return apiResources;
        }

        /// <summary>
        /// Get identity resources
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<IdentityResource>> GetIdentityResourcesAsync()
        {
            var identityApiResources = await _cacheService.GetAsync<IEnumerable<IdentityResource>>(IdentityResourceCacheKey);
            if (identityApiResources != null) return identityApiResources.ToList();

            var identityResources = await _clientsContext.IdentityResources
                .Include(x => x.Properties)
                .Include(x => x.UserClaims)
                .ToListAsync();
            await _cacheService.SetAsync(IdentityResourceCacheKey, identityResources);
            return identityResources;
        }

        /// <summary>
        /// Find api resource by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<ApiResource>> FindApiResourceByIdAsync(int id)
        {
            var apiResources = await GetApiResourcesAsync();
            var apiResource = apiResources?.FirstOrDefault(x => x.Id.Equals(id));
            if (apiResource == null) return new NotFoundResultModel<ApiResource>();
            return new SuccessResultModel<ApiResource>(apiResource);
        }

        /// <summary>
        /// Update api resource
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> UpdateApiResourceAsync(ApiResource model)
        {
            var data = await _clientsContext.ApiResources.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (data == null) return new InvalidParametersResultModel();

            data.Name = model.Name;
            data.Enabled = model.Enabled;
            data.Description = model.Description;

            _clientsContext.ApiResources.Update(data);
            var dbResult = await _clientsContext.PushAsync();
            if (dbResult.IsSuccess) await ResetCacheAsync();
            return dbResult;
        }

        /// <summary>
        /// Disable api resource
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> DisableApiResourceAsync(int id)
            => await ToggleActiveStatusApiResourceAsync(id, false);

        /// <summary>
        /// Enable api resource
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> EnableApiResourceAsync(int id)
            => await ToggleActiveStatusApiResourceAsync(id, true);

        /// <summary>
        /// Toggle active status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        protected virtual async Task<ResultModel> ToggleActiveStatusApiResourceAsync(int id, bool state)
        {
            var data = await _clientsContext.ApiResources.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null) return new InvalidParametersResultModel();
            data.Enabled = state;
            _clientsContext.ApiResources.Update(data);
            var dbResult = await _clientsContext.PushAsync();
            if (dbResult.IsSuccess) await ResetCacheAsync();
            return dbResult;
        }

        /// <summary>
        /// Reset cache
        /// </summary>
        /// <returns></returns>
        private async Task ResetCacheAsync()
        {
            await _cacheService.RemoveAsync(ClientsCacheKey);
            await _cacheService.RemoveAsync(ApiResourceCacheKey);
        }
    }
}