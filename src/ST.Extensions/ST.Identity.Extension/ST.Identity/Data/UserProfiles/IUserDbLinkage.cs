using Microsoft.AspNetCore.Identity;

namespace ST.Identity.Data.UserProfiles
{
    /// <summary>
    /// Contract that ensures that there is a relationship with the 
    /// implementation model and the User Identity
    /// </summary>
    /// <typeparam name="TUserKey">Type of the User's Id Property, usually <see cref="string"/></typeparam>
    /// <typeparam name="TUser">The type of the User Identity Model</typeparam>
	public interface IUserDbLinkage<TUserKey, TUser> where TUser: IdentityUser
    {
		TUserKey UserId { get; set; }
		TUser User { get; set; }
    }
}
