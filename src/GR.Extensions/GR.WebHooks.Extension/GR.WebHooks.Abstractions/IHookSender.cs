using System.Threading.Tasks;
using GR.Core.Events.EventArgs;

namespace GR.WebHooks.Abstractions
{
    public interface IHookSender
    {
        /// <summary>
        /// Send to provider
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="evt"></param>
        /// <returns></returns>
        Task SendAsync(string providerName, ApplicationEventEventArgs evt);
    }
}