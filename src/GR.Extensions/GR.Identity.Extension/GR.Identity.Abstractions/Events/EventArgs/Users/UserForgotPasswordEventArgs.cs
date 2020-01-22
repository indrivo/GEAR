namespace GR.Identity.Abstractions.Events.EventArgs.Users
{
    public class UserForgotPasswordEventArgs : System.EventArgs
    {
        public string Email { get; set; }
    }
}