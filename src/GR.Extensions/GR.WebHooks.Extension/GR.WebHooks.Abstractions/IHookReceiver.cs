using System;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.WebHooks.Abstractions.Models;
using Microsoft.AspNetCore.Http;

namespace GR.WebHooks.Abstractions
{
    public interface IHookReceiver
    {
        Task<ResultModel> ReceiveEventAsync(Guid? hookId, HttpContext httpContext);

        ResultModel ValidateReceiver(WebHook hook, HttpContext httpContext);
    }
}