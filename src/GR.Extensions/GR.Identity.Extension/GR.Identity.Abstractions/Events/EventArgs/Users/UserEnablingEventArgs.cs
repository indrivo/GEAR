namespace GR.Identity.Abstractions.Events.EventArgs.Users
{
    public class UserEnablingEventArgs : System.EventArgs
    {
        public string UserId { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsDeleted { get; set; }
    }
}