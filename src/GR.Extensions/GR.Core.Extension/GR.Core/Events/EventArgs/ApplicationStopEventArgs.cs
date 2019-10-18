using System;

namespace GR.Core.Events.EventArgs
{
    public class ApplicationStopEventArgs : System.EventArgs
    {
        public IServiceProvider Services { get; set; }
        public string AppIdentifier { get; set; }
    }
}
