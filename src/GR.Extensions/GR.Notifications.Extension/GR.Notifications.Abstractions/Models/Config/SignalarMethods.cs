using System;
using System.Collections.Generic;

namespace GR.Notifications.Abstractions.Models.Config
{
    /// <summary>
    /// Signal connection
    /// </summary>
    public class SignalrConnection
    {
        public string ConnectionId { get; set; }
        public Guid UserId { get; set; }
    }

    public static class SignalrSendMethods
    {
        public const string SendClientNotification = "SendClientNotification";
        public const string SendData = "SendData";
    }
}
