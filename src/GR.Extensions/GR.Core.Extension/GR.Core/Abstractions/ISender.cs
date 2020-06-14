using System.Threading.Tasks;
using GR.Core.Helpers;

namespace GR.Core.Abstractions
{
    public interface ISender
    {
        /// <summary>
        /// Send async
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        Task<ResultModel> SendAsync(string subject, string message, string to);

        /// <summary>
        /// Get provider
        /// </summary>
        /// <returns></returns>
        object GetProvider();
    }
}