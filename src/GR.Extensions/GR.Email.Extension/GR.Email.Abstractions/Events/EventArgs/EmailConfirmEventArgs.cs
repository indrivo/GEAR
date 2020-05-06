using Microsoft.AspNetCore.Http;

namespace GR.Email.Abstractions.Events.EventArgs
{
    public class EmailConfirmEventArgs : System.EventArgs
    {
        public virtual HttpContext HttpContext { get; set; }
        public virtual string Email { get; set; }
        public virtual bool IsConfirmed { get; set; }
    }
}