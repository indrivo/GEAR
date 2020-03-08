using System;
using System.Threading.Tasks;
using GR.Notifications.Abstractions.Models.Config;
using GR.Notifications.Hub.Helpers;

namespace GR.Notifications.Hub.Hubs
{
    public class GearNotificationHub : Microsoft.AspNetCore.SignalR.Hub
    {
        internal static class UserConnections
        {
            /// <summary>
            /// Store connections on memory
            /// </summary>
            public static readonly ConnectionMapping Connections = new ConnectionMapping();
        }

        /// <summary>
        /// On web app load
        /// </summary>
        /// <returns></returns>
        public Task OnLoad(Guid id)
        {
            UserConnections.Connections.Add(new SignalrConnection
            {
                ConnectionId = Context.ConnectionId,
                UserId = id
            });
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
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
