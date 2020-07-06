using System;

namespace GR.UserPreferences.Abstractions.Events.EventArgs
{
    public class KeyUpdateEventArgs : System.EventArgs
    {
        public virtual Guid UserId { get; set; }
        public virtual string KeyName { get; set; }
        public virtual string NewValue { get; set; }
    }
}