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
            FirstName = user.UserFirstName;
            LastName = user.UserLastName;
            Email = user.Email;
        }

        public virtual string Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
    }
}