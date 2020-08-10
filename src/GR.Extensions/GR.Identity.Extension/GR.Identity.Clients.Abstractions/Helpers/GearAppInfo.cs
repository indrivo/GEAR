using System.Threading.Tasks;
using GR.Core.Abstractions;

namespace GR.Identity.Clients.Abstractions.Helpers
{
    public class GearAppInfo : IGearAppInfo
    {
        #region Injectable

        private readonly IClientsService _clientsService;

        #endregion


        public GearAppInfo(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        public async Task<string> GetAppNameAsync(string client)
        {
            return await _clientsService.GetAppName(client);
        }
    }
}
