using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ST.Notifications.Abstractions.Models.Config;

namespace ST.Notifications.Hubs
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NotificationsHub : Hub
    {
        /// <summary>
        /// Store connections on memory
        /// </summary>
        public static readonly ConnectionMapping Connections = new ConnectionMapping();

        /// <summary>
        /// On web app load
        /// </summary>
        /// <returns></returns>
        public Task OnLoad(Guid id)
        {
            Connections.Add(new SignalrConnection
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
            Connections.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.Name)?.Value;
        }
    }
}
