namespace GR.Identity.Abstractions.Events.EventArgs.Users
{
    public class UserCreatedEventArgs : System.EventArgs
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}