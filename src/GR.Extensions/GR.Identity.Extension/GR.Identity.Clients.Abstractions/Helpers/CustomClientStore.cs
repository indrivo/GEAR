using System;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;

namespace GR.Identity.Clients.Abstractions.Helpers
{
    public class CustomClientStore : IClientStore
    {
        #region Injectable

        /// <summary>
        /// Inject clients context
        /// </summary>
        private readonly IClientsService _clientsService;

        /// <summary>
        /// Inject logger
        /// </summary>
        private readonly ILogger<CustomClientStore> _logger;

        #endregion

        public CustomClientStore(IClientsService clientsService, ILogger<CustomClientStore> logger)
        {
            _clientsService = clientsService;
            _logger = logger;
        }

        /// <summary>
        /// Find client by id
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var clients = await _clientsService.GetAllClientsAsync();
            var client = clients?.FirstOrDefault(x => x.ClientId.Equals(clientId));
            try
            {
                return client?.ToModel();
            }
            catch (Exception mapError)
            {
                _logger.LogError(mapError, mapError.Message);
                _logger.LogError($"Fail to map client, client: {client.SerializeAsJson()}");
                try
                {
                    clients = await _clientsService.GetAllClientsAsync(true);
                    return clients?.FirstOrDefault(x => x.ClientId.Equals(clientId))?.ToModel();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return default;
            }
        }
    }
}
