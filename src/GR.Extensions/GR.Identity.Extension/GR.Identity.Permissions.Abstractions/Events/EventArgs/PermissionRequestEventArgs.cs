namespace GR.Identity.Permissions.Abstractions.Events.EventArgs
{
    public sealed class PermissionRequestEventArgs : System.EventArgs
    {
        public string PermissionName { get; set; }
    }
}
