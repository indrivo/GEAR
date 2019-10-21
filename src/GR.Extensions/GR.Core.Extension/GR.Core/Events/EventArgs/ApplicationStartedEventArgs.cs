using System;

namespace GR.Core.Events.EventArgs
{
    public class ApplicationStartedEventArgs : System.EventArgs
    {
        public IServiceProvider Services { get; set; }
        public string AppIdentifier { get; set; }
    }
}
