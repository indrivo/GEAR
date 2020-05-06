using Microsoft.AspNetCore.Http;

namespace GR.Email.Abstractions.Events.EventArgs
{
    public class SendConfirmEmailEventArgs : System.EventArgs
    {
        public virtual HttpContext HttpContext { get; set; }
        public virtual string Email { get; set; }
    }
}
