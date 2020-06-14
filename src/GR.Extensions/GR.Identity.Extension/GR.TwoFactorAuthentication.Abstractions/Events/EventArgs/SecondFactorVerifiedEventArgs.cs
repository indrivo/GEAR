using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace GR.TwoFactorAuthentication.Abstractions.Events.EventArgs
{
    public class SecondFactorVerifiedEventArgs : System.EventArgs
    {
        public virtual Guid UserId { get; set; }
        public virtual HttpContext HttpContext { get; set; }
        public virtual Dictionary<string, string> Data { get; set; }
        public virtual string AuthMethod { get; set; }
    }
}