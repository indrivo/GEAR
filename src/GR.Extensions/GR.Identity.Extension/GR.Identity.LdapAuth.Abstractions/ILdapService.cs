using GR.Identity.LdapAuth.Abstractions.Models;
using System.Collections.Generic;
using GR.Core.Helpers;

namespace GR.Identity.LdapAuth.Abstractions
{
    public interface ILdapService<TUser> where TUser : LdapUser
    {
        /// <summary>
        /// Get groups
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="getChildGroups"></param>
        /// <returns></returns>
        ICollection<LdapEntry> GetGroups(string groupName, bool getChildGroups = false);

        /// <summary>
        /// Get user in group
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>

        ICollection<TUser> GetUsersInGroup(string groupName);

        /// <summary>
        /// Get users in group
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        List<TUser> GetUsersInGroups(ICollection<LdapEntry> groups = null);

        /// <summary>
        /// Get users by email
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        ICollection<TUser> GetUsersByEmailAddress(string emailAddress);

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns></returns>
        IEnumerable<TUser> GetAllUsers();

        /// <summary>
        /// Get administrator
        /// </summary>
        /// <returns></returns>
        TUser GetAdministrator();

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        TUser GetUserByUserName(string userName);

        /// <summary>
        /// Add new user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        void AddUser(TUser user, string password);

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="distinguishedName"></param>
        void DeleteUser(string distinguishedName);

        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="distinguishedName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        ResultModel Authenticate(string distinguishedName, string password);
    }
}