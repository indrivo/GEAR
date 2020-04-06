using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
using IdentityServer4.EntityFramework.Entities;

namespace GR.Identity.Clients.Abstractions
{
    public interface IClientsService
    {
        /// <summary>
        /// Get app id
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task<string> GetAppName(string appId);

        /// <summary>
        /// Get all clients
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Client>> GetAllClientsAsync();

        /// <summary>
        /// Get api resources
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ApiResource>> GetApiResourcesAsync();

        /// <summary>
        /// Get identity resources
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<IdentityResource>> GetIdentityResourcesAsync();

        /// <summary>
        /// Find api resource by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel<ApiResource>> FindApiResourceByIdAsync(int id);

        /// <summary>
        /// Update api resource
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateApiResourceAsync(ApiResource model);

        /// <summary>
        /// Disable api resource
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel> DisableApiResourceAsync(int id);

        /// <summary>
        /// Enable api resource
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel> EnableApiResourceAsync(int id);
    }
}