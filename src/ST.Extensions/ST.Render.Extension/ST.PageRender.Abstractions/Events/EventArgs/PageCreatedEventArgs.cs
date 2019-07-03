using System;

namespace ST.PageRender.Abstractions.Events.EventArgs
{
    public class PageCreatedEventArgs : System.EventArgs
    {
        public Guid PageId { get; set; }
        public string PageName { get; set; }
    }
}
