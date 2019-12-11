using System;
using System.Collections.Generic;

namespace GR.Identity.Razor.Users.ViewModels.UserViewModels
{
    public class UserListItemViewModel
    {
        public UserListItemViewModel()
        {
            Roles = new List<string>();
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string Changed { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public int Sessions { get; set; }
        public string AuthenticationType { get; set; }
        public string Organization { get; set; }
        public DateTime LastLogin { get; set; }
        public int ExpireDays { get; set; }
    }
}