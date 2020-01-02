namespace GR.Identity.Abstractions.Events.EventArgs.Users
{
    public class UserEmailConfirmEventArgs : System.EventArgs
    {
        public string Email { get; set; }
        public string UserId { get; set; }
    }
}