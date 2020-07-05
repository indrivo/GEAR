using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Events.EventArgs;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.WebHooks.Abstractions.Enums;
using GR.WebHooks.Abstractions.Helpers;
using GR.WebHooks.Abstractions.Models;
using GR.WebHooks.Abstractions.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GR.WebHooks.Abstractions.Services
{
    public class DefaultHookService : IIncomingHookService, IOutgoingHookService
    {
        #region Injectable

        /// <summary>
        /// inject context
        /// </summary>
        private readonly IWebHookContext _context;

        #endregion

        public DefaultHookService(IWebHookContext context)
        {
            _context = context;
        }

        public async Task SendEventAsync(ApplicationEventEventArgs evt)
        {
            var hooks = await GetHooksByDirectionAndEventAsync(evt.EventName, HookDirection.Outgoing);
            foreach (var hook in hooks)
            {

            }
        }

        /// <summary>
        /// Get hooks by event name and direction
        /// </summary>
        /// <param name="evtName"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public async Task<IEnumerable<WebHook>> GetHooksByDirectionAndEventAsync(string evtName, HookDirection direction)
        {
            var hooks = await _context.WebHooks
                .NonDeleted()
                .Include(x => x.Provider)
                .Include(x => x.Events)
                .Where(x => x.Direction == direction && x.Events.Select(y => y.EventName).Contains(evtName))
                .ToListAsync();
            return hooks;
        }

        /// <summary>
        /// Receive event
        /// </summary>
        /// <param name="hook"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task<ResultModel> ReceiveEventAsync(IncomingHookRequestViewModel hook, HttpContext httpContext)
        {
            var providerRequest = ProviderHookHelper.ResolveIncomingProvider(hook.ProviderId);
            if (!providerRequest.IsSuccess) return providerRequest.ToBase();
            var provider = providerRequest.Result;
            return await provider.ReceiveEventAsync(hook.HookId, httpContext);
        }
    }
}