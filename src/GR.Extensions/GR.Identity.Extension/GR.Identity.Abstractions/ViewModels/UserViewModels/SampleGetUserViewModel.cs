using System;

namespace GR.Identity.Abstractions.ViewModels.UserViewModels
{
    public class SampleGetUserViewModel
    {
        public SampleGetUserViewModel()
        {
        }

        public SampleGetUserViewModel(GearUser user)
        {
            if (user == null) return;
            Id = user.Id;
            UserName = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
        }

        public virtual Guid Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
    }
}