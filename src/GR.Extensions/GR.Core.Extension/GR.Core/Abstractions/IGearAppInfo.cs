using System.Threading.Tasks;

namespace GR.Core.Abstractions
{
    public interface IGearAppInfo
    {
        /// <summary>
        /// Get app name
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        Task<string> GetAppNameAsync(string client);
    }
}