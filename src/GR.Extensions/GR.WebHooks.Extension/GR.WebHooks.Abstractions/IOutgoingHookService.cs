using System.Threading.Tasks;
using GR.Core.Events.EventArgs;

namespace GR.WebHooks.Abstractions
{
    public interface IOutgoingHookService
    {
        /// <summary>
        /// Process
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        Task SendEventAsync(ApplicationEventEventArgs evt);
    }
}