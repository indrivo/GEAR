using System.Collections.Generic;

namespace GR.Identity.Permissions.Abstractions.Events.EventArgs
{
    public class PermissionsSeedEventsArgs : System.EventArgs
    {
        public Dictionary<string, string> Permissions { get; set; }
    }
}