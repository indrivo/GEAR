using System;
using GR.Core;

namespace GR.Identity.Abstractions.ViewModels.UserViewModels
{
    public class UserInfoViewModel : BaseModel
    {
        public UserInfoViewModel()
        {
        }

        public UserInfoViewModel(GearUser user)
        {
            if (user == null) return;
            Id = user.Id;
            UserName = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
        }

        public virtual string UserName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual DateTime Birthday { get; set; }
        public virtual DateTime LastLogin { get; set; }
        public virtual string AboutMe { get; set; }
        public virtual bool EmailConfirmed { get; set; }
        public virtual bool PhoneNumberConfirmed { get; set; }
        public virtual string PhoneNumber { get; set; }
    }
}