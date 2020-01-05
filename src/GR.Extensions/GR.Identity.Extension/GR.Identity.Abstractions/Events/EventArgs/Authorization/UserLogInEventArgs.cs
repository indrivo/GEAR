using System;

namespace GR.Identity.Abstractions.Events.EventArgs.Authorization
{
    public class UserLogInEventArgs : System.EventArgs
    {
        public string IpAdress { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}