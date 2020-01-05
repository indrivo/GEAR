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

    public class SignalrEmail
    {
        public Guid UserId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public List<string> EmailRecipients { get; set; }
    }

    public abstract class SignalrSendMethods
    {
        public const string SendClientEmail = "SendClientEmailNotification";
        public const string SendClientNotification = "SendClientNotification";
    }
}
