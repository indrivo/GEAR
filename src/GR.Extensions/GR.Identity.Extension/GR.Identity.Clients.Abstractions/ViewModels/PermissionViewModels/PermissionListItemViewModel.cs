using System;

namespace GR.Identity.Clients.Abstractions.ViewModels.PermissionViewModels
{
    public class PermissionListItemViewModel
    {
        public Guid Id { get; set; }
        public string ClientName { get; set; }
        public string PermissionName { get; set; }
        public string PermissionDescription { get; set; }
        public string PermissionKey { get; set; }
    }
}