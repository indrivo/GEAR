using System;

namespace GR.Subscriptions.Abstractions.Events.EventArgs
{
    public class UpgradeSubscriptionEventArgs : System.EventArgs
    {
        public virtual Guid SubscriptionId { get; set; }
        public virtual Guid UserId { get; set; }
    }
}