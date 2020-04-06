using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace GR.Identity.Clients.Abstractions.Helpers
{
    public class CustomClientStore : IClientStore
    {
        #region Injectable

        /// <summary>
        /// Inject clients context
        /// </summary>
        private readonly IClientsService _clientsService;

        #endregion

        public CustomClientStore(IClientsService clientsService)
        {
            _clientsService = clientsService;
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
            return client?.ToModel();
        }
    }
}
