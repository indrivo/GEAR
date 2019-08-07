using System.Threading.Tasks;

namespace ST.Identity.Abstractions
{
    public interface IAppProvider
    {
        /// <summary>
        /// Get app id
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task<string> GetAppName(string appId);
    }
}
