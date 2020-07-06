using System;
using GR.Core;

namespace GR.Identity.Abstractions.ViewModels.UserViewModels
{
    public class UserInfoViewModel : BaseModel
    {
        /// <summary>
        /// User full name
        /// </summary>
        public virtual string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// User name
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        public virtual string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public virtual string LastName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Birth date
        /// </summary>
        public virtual DateTime Birthday { get; set; }

        /// <summary>
        /// last log in date
        /// </summary>
        public virtual DateTime LastLogin { get; set; }

        /// <summary>
        /// About user
        /// </summary>
        public virtual string AboutMe { get; set; }

        /// <summary>
        /// Is email confirmed
        /// </summary>
        public virtual bool EmailConfirmed { get; set; }

        /// <summary>
        /// Is phone confirmed
        /// </summary>
        public virtual bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Phone number
        /// </summary>
        public virtual string PhoneNumber { get; set; }
    }
}