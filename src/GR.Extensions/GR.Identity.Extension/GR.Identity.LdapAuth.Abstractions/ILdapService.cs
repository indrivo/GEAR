using GR.Identity.LdapAuth.Abstractions.Models;
using System.Collections.Generic;

namespace GR.Identity.LdapAuth.Abstractions
{
    public interface ILdapService<TUser> where TUser : LdapUser
    {
        ICollection<LdapEntry> GetGroups(string groupName, bool getChildGroups = false);

        ICollection<TUser> GetUsersInGroup(string groupName);

        List<TUser> GetUsersInGroups(ICollection<LdapEntry> groups = null);

        ICollection<TUser> GetUsersByEmailAddress(string emailAddress);

        IEnumerable<TUser> GetAllUsers();

        TUser GetAdministrator();

        TUser GetUserByUserName(string userName);

        void AddUser(TUser user, string password);

        void DeleteUser(string distinguishedName);

        bool Authenticate(string distinguishedName, string password);
    }
}