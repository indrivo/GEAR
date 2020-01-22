namespace GR.Identity.Abstractions.Events.EventArgs.Users
{
    public class UserChangePasswordEventArgs : UserCreatedEventArgs
    {
        public string Password { get; set; }
    }
}