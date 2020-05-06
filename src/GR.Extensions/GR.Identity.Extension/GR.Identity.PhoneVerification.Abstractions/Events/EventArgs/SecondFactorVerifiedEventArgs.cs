using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace GR.Identity.PhoneVerification.Abstractions.Events.EventArgs
{
    public class SecondFactorVerifiedEventArgs : System.EventArgs
    {
        public virtual HttpContext HttpContext { get; set; }
        public virtual Dictionary<string, string> Data { get; set; }
    }
}
