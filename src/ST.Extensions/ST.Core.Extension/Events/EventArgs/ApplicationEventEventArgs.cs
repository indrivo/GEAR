using System;
using System.Collections.Generic;

namespace ST.Core.Events.EventArgs
{
    public class ApplicationEventEventArgs : System.EventArgs
    {
        public Dictionary<string, object> EventArgs { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
    }
}
