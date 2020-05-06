using Microsoft.AspNetCore.Http;

namespace GR.Identity.PhoneVerification.Abstractions.Events.EventArgs
{
    public class PhoneConfirmedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Context
        /// </summary>
        public virtual HttpContext HttpContext { get; set; }

        /// <summary>
        /// Phone
        /// </summary>
        public virtual string PhoneNumber { get; set; }
    }
}