using System.Threading.Tasks;

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
    }
}