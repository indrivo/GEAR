using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.WebHooks.Abstractions.ViewModels;
using Microsoft.AspNetCore.Http;

namespace GR.WebHooks.Abstractions
{
    public interface IIncomingHookService
    {
        /// <summary>
        /// Receive event
        /// </summary>
        /// <param name="hook"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        Task<ResultModel> ReceiveEventAsync(IncomingHookRequestViewModel hook, HttpContext httpContext);
    }
}