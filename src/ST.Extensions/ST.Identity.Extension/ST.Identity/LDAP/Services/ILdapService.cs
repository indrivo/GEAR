using System.Collections.Generic;
using ST.Identity.Abstractions;
using ST.Identity.Abstractions.Ldap.Models;
using ST.Identity.Data.UserProfiles;

namespace ST.Identity.LDAP.Services
{
    public interface ILdapService
    {
        ICollection<LdapEntry> GetGroups(string groupName, bool getChildGroups = false);

        ICollection<ApplicationUser> GetUsersInGroup(string groupName);

        List<ApplicationUser> GetUsersInGroups(ICollection<LdapEntry> groups = null);

        ICollection<ApplicationUser> GetUsersByEmailAddress(string emailAddress);

        IEnumerable<ApplicationUser> GetAllUsers();

        ApplicationUser GetAdministrator();

        ApplicationUser GetUserByUserName(string userName);

        void AddUser(ApplicationUser user, string password);

        void DeleteUser(string distinguishedName);

        bool Authenticate(string distinguishedName, string password);
    }
}
