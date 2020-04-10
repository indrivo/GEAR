using System;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Identity.Abstractions.Extensions;
using GR.Notifications.Abstractions.Models.Config;
using GR.Notifications.Hub.Helpers;
using Microsoft.AspNetCore.Http;

namespace GR.Notifications.Hub.Hubs
{
    public class GearNotificationHub : Microsoft.AspNetCore.SignalR.Hub
    {
        /// <summary>
        /// Inject http context
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GearNotificationHub(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        internal static class UserConnections
        {
            /// <summary>
            /// Store connections on memory
            /// </summary>
            public static readonly ConnectionMapping Connections = new ConnectionMapping();
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {

            await base.OnConnectedAsync();
            if (_httpContextAccessor?.HttpContext?.User.IsAuthenticated() ?? false)
            {
                var userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
                UserConnections.Connections.Add(new SignalrConnection
                {
                    ConnectionId = Context.ConnectionId,
                    UserId = userId.ToGuid()
                });
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// On User disconnect
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            UserConnections.Connections.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
