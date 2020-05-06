using System;
using GR.AccountActivity.Abstractions.Models;
using Microsoft.AspNetCore.Http;

namespace GR.AccountActivity.Abstractions.Events.EventArgs
{
    public class DeviceConfirmedEventArgs : System.EventArgs
    {
        public virtual HttpContext HttpContext { get; set; }
        public virtual Guid DeviceId { get; set; }
        public virtual UserDevice Device { get; set; }
    }
}
