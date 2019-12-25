using GR.Identity.LdapAuth.Abstractions;
using GR.Identity.LdapAuth.Abstractions.Models;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Principal;
using System.Text;
using LdapEntry = GR.Identity.LdapAuth.Abstractions.Models.LdapEntry;

namespace GR.Identity.LdapAuth
{
    public class LdapService<TUser> : ILdapService<TUser> where TUser : LdapUser, new()
    {
        /// <summary>
        /// Search base
        /// </summary>
        private readonly string _searchBase;

        /// <summary>
        /// Inject LdapSettings
        /// </summary>
        private readonly LdapSettings _ldapSettings;

        /// <summary>
        /// Attributes
        /// </summary>
        private readonly string[] _attributes =
        {
            "objectSid", "objectGUID", "objectCategory", "objectClass", "memberOf", "name", "cn", "distinguishedName",
            "sAMAccountName", "sAMAccountName", "userPrincipalName", "displayName", "givenName", "sn", "description",
            "telephoneNumber", "mail", "streetAddress", "postalCode", "l", "st", "co", "c"
        };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ldapSettingsOptions"></param>
        public LdapService(IOptions<LdapSettings> ldapSettingsOptions)
        {
            _ldapSettings = ldapSettingsOptions.Value;
            _searchBase = _ldapSettings.SearchBase;
        }

        /// <summary>
        /// Get Connection
        /// </summary>
        /// <returns></returns>
        protected virtual ILdapConnection GetConnection()
        {
            var ldapConnection = new LdapConnection() { SecureSocketLayer = _ldapSettings.UseSSL };
            try
            {
                //Connect function will create a socket connection to the server - Port 389 for insecure and 3269 for secure
                ldapConnection.Connect(_ldapSettings.ServerName, _ldapSettings.ServerPort);
                //Bind function with null user dn and password value will perform anonymous bind to LDAP server
                ldapConnection.Bind(_ldapSettings.Credentials.DomainUserName, _ldapSettings.Credentials.Password);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return ldapConnection;
        }

        /// <summary>
        /// Get groups
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="getChildGroups"></param>
        /// <returns></returns>
        public virtual ICollection<LdapEntry> GetGroups(string groupName, bool getChildGroups = false)
        {
            var groups = new Collection<LdapEntry>();

            var filter = $"(&(objectClass=group)(cn={groupName}))";

            using (var ldapConnection = GetConnection())
            {
                var search = ldapConnection.Search(
                    _searchBase,
                    LdapConnection.SCOPE_SUB,
                    filter,
                    _attributes,
                    false,
                    null,
                    null);

                LdapMessage message;

                while ((message = search.getResponse()) != null)
                {
                    if (!(message is LdapSearchResult searchResultMessage))
                    {
                        continue;
                    }

                    var entry = searchResultMessage.Entry;

                    groups.Add(CreateEntryFromAttributes(entry.DN, entry.getAttributeSet()));

                    if (!getChildGroups)
                    {
                        continue;
                    }

                    foreach (var child in GetChildren<LdapEntry>(string.Empty, entry.DN))
                    {
                        groups.Add(child);
                    }
                }
            }

            return groups;
        }

        /// <summary>
        /// Get All Users
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TUser> GetAllUsers()
        {
            return GetUsersInGroups(null);
        }

        /// <summary>
        /// Get Users In Group
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public virtual ICollection<TUser> GetUsersInGroup(string group)
        {
            return GetUsersInGroups(GetGroups(group));
        }

        /// <summary>
        /// Get Users In Groups
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        public virtual List<TUser> GetUsersInGroups(ICollection<LdapEntry> groups)
        {
            var users = new List<TUser>();

            if (groups == null || !groups.Any())
            {
                users.AddRange(GetChildren<TUser>(_searchBase));
            }
            else
            {
                foreach (var group in groups)
                {
                    users.AddRange(GetChildren<TUser>(_searchBase, @group.DistinguishedName));
                }
            }

            return users;
        }

        /// <summary>
        /// Get Users By Email Address
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public virtual ICollection<TUser> GetUsersByEmailAddress(string emailAddress)
        {
            var users = new Collection<TUser>();

            var filter = $"(&(objectClass=user)(mail={emailAddress}))";

            using (var ldapConnection = GetConnection())
            {
                var search = ldapConnection.Search(
                    _searchBase,
                    LdapConnection.SCOPE_SUB,
                    filter,
                    _attributes,
                    false, null, null);

                LdapMessage message;

                while ((message = search.getResponse()) != null)
                {
                    if (!(message is LdapSearchResult searchResultMessage))
                    {
                        continue;
                    }

                    users.Add(CreateUserFromAttributes(_searchBase,
                        searchResultMessage.Entry.getAttributeSet()));
                }
            }

            return users;
        }

        /// <summary>
        /// Get user by user name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public virtual TUser GetUserByUserName(string userName)
        {
            TUser user = null;

            var filter = $"(&(objectClass=user)(samAccountName={userName}))";

            using (var ldapConnection = GetConnection())
            {
                try
                {
                    var search = ldapConnection.Search(
                        _searchBase,
                        LdapConnection.SCOPE_SUB,
                        filter,
                        _attributes,
                        false,
                        null,
                        null);

                    LdapMessage message;

                    while ((message = search.getResponse()) != null)
                    {
                        if (!(message is LdapSearchResult searchResultMessage))
                        {
                            continue;
                        }

                        user = CreateUserFromAttributes(_searchBase, searchResultMessage.Entry.getAttributeSet());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return user;
        }

        /// <summary>
        /// Get administrator
        /// </summary>
        /// <returns></returns>
        public virtual TUser GetAdministrator()
        {
            var name = _ldapSettings.Credentials.DomainUserName.Substring(
                _ldapSettings.Credentials.DomainUserName.IndexOf("\\", StringComparison.Ordinal) != -1
                    ? _ldapSettings.Credentials.DomainUserName.IndexOf("\\", StringComparison.Ordinal) + 1
                    : 0);

            return GetUserByUserName(name);
        }

        /// <summary>
        /// Add user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public virtual void AddUser(TUser user, string password)
        {
            var dn = $"CN={user.FirstName} {user.LastName},{_ldapSettings.ContainerName}";

            var attributeSet = new LdapAttributeSet
            {
                new LdapAttribute("instanceType", "4"),
                new LdapAttribute("objectCategory",
                    $"CN=Person,CN=Schema,CN=Configuration,{_ldapSettings.DomainDistinguishedName}"),
                new LdapAttribute("objectClass", new[] {"top", "person", "organizationalPerson", "user"}),
                new LdapAttribute("name", user.Name),
                new LdapAttribute("cn", $"{user.FirstName} {user.LastName}"),
                new LdapAttribute("sAMAccountName", user.Name),
                new LdapAttribute("userPrincipalName", user.Name),
                new LdapAttribute("unicodePwd",
                    Convert.ToBase64String(Encoding.Unicode.GetBytes($"\"{user.Password}\""))),
                new LdapAttribute("userAccountControl", user.MustChangePasswordOnNextLogon ? "544" : "512"),
                new LdapAttribute("givenName", user.FirstName),
                new LdapAttribute("sn", user.LastName),
                new LdapAttribute("mail", user.EmailAddress)
            };

            if (user.DisplayName != null)
            {
                attributeSet.Add(new LdapAttribute("displayName", user.DisplayName));
            }

            if (user.Description != null)
            {
                attributeSet.Add(new LdapAttribute("description", user.Description));
            }

            if (user.Phone != null)
            {
                attributeSet.Add(new LdapAttribute("telephoneNumber", user.Phone));
            }

            if (user.Address?.Street != null)
            {
                attributeSet.Add(new LdapAttribute("streetAddress", user.Address.Street));
            }

            if (user.Address?.City != null)
            {
                attributeSet.Add(new LdapAttribute("l", user.Address.City));
            }

            if (user.Address?.PostalCode != null)
            {
                attributeSet.Add(new LdapAttribute("postalCode", user.Address.PostalCode));
            }

            if (user.Address?.StateName != null)
            {
                attributeSet.Add(new LdapAttribute("st", user.Address.StateName));
            }

            if (user.Address?.CountryName != null)
            {
                attributeSet.Add(new LdapAttribute("co", user.Address.CountryName));
            }

            if (user.Address?.CountryCode != null)
            {
                attributeSet.Add(new LdapAttribute("c", user.Address.CountryCode));
            }

            var newEntry = new Novell.Directory.Ldap.LdapEntry(dn, attributeSet);

            using (var ldapConnection = GetConnection())
            {
                ldapConnection.Add(newEntry);
            }
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="distinguishedName"></param>
        public virtual void DeleteUser(string distinguishedName)
        {
            using (var ldapConnection = GetConnection())
            {
                ldapConnection.Delete(distinguishedName);
            }
        }

        /// <summary>
        /// Authentication
        /// </summary>
        /// <param name="distinguishedName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public virtual bool Authenticate(string distinguishedName, string password)
        {
            using (var ldapConnection = new LdapConnection() { SecureSocketLayer = _ldapSettings.UseSSL })
            {
                try
                {
                    ldapConnection.Connect(_ldapSettings.ServerName, _ldapSettings.ServerPort);
                    ldapConnection.Bind(distinguishedName, password);

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Get children
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchBase"></param>
        /// <param name="groupDistinguishedName"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedParameter.Local
        protected virtual IList<T> GetChildren<T>(string searchBase, string groupDistinguishedName = null)
            where T : ILdapEntry, new()
        {
            var entries = new List<T>();

            string objectCategory;
            string objectClass;

            if (typeof(T) == typeof(LdapEntry))
            {
                objectClass = "group";
                objectCategory = "group";

                entries = GetChildren(_searchBase, groupDistinguishedName, objectCategory, objectClass)
                    .Cast<T>().ToList();
            }

            if (typeof(T) != typeof(TUser)) return entries;
            objectCategory = "person";
            objectClass = "user";

            entries = GetChildren(_searchBase, null, objectCategory, objectClass).Cast<T>().ToList();

            return entries;
        }

        /// <summary>
        /// Get children
        /// </summary>
        /// <param name="searchBase"></param>
        /// <param name="groupDistinguishedName"></param>
        /// <param name="objectCategory"></param>
        /// <param name="objectClass"></param>
        /// <returns></returns>
        protected virtual IEnumerable<ILdapEntry> GetChildren(string searchBase, string groupDistinguishedName = null,
            string objectCategory = "*", string objectClass = "*")
        {
            var allChildren = new Collection<ILdapEntry>();

            var filter = string.IsNullOrEmpty(groupDistinguishedName)
                ? $"(&(objectCategory={objectCategory})(objectClass={objectClass}))"
                : $"(&(objectCategory={objectCategory})(objectClass={objectClass})(memberOf={groupDistinguishedName}))";

            using (var ldapConnection = GetConnection())
            {
                var search = ldapConnection.Search(
                    searchBase,
                    LdapConnection.SCOPE_SUB,
                    filter,
                    _attributes,
                    false,
                    null,
                    null);

                LdapMessage message;

                while ((message = search.getResponse()) != null)
                {
                    if (!(message is LdapSearchResult searchResultMessage))
                    {
                        continue;
                    }

                    var entry = searchResultMessage.Entry;

                    switch (objectClass)
                    {
                        case "group":
                            {
                                allChildren.Add(CreateEntryFromAttributes(entry.DN, entry.getAttributeSet()));

                                foreach (var child in GetChildren(string.Empty, entry.DN, objectCategory, objectClass))
                                {
                                    allChildren.Add(child);
                                }

                                break;
                            }
                        case "user":
                            allChildren.Add(CreateUserFromAttributes(entry.DN, entry.getAttributeSet()));
                            break;
                    }
                }
            }

            return allChildren;
        }

        /// <summary>
        /// Create User From Attributes
        /// </summary>
        /// <param name="distinguishedName"></param>
        /// <param name="attributeSet"></param>
        /// <returns></returns>
        protected virtual TUser CreateUserFromAttributes(string distinguishedName, LdapAttributeSet attributeSet)
        {
            var ldapUser = new TUser
            {
                ObjectSid = attributeSet.getAttribute("objectSid")?.StringValue,
                ObjectGuid = attributeSet.getAttribute("objectGUID")?.StringValue,
                ObjectCategory = attributeSet.getAttribute("objectCategory")?.StringValue,
                ObjectClass = attributeSet.getAttribute("objectClass")?.StringValue,
                IsDomainAdmin = attributeSet.getAttribute("memberOf") != null && attributeSet.getAttribute("memberOf")
                                    .StringValueArray.Contains("CN=Domain Admins," + _ldapSettings.SearchBase),
                MemberOf = attributeSet.getAttribute("memberOf")?.StringValueArray,
                CommonName = attributeSet.getAttribute("cn")?.StringValue,
                UserName = attributeSet.getAttribute("name")?.StringValue,
                SamAccountName = attributeSet.getAttribute("sAMAccountName")?.StringValue,
                UserPrincipalName = attributeSet.getAttribute("userPrincipalName")?.StringValue,
                Name = attributeSet.getAttribute("name")?.StringValue,
                DistinguishedName = attributeSet.getAttribute("distinguishedName")?.StringValue ?? distinguishedName,
                DisplayName = attributeSet.getAttribute("displayName")?.StringValue,
                FirstName = attributeSet.getAttribute("givenName")?.StringValue,
                LastName = attributeSet.getAttribute("sn")?.StringValue,
                Description = attributeSet.getAttribute("description")?.StringValue,
                Phone = attributeSet.getAttribute("telephoneNumber")?.StringValue,
                EmailAddress = attributeSet.getAttribute("mail")?.StringValue,
                Address = new LdapAddress
                {
                    Street = attributeSet.getAttribute("streetAddress")?.StringValue,
                    City = attributeSet.getAttribute("l")?.StringValue,
                    PostalCode = attributeSet.getAttribute("postalCode")?.StringValue,
                    StateName = attributeSet.getAttribute("st")?.StringValue,
                    CountryName = attributeSet.getAttribute("co")?.StringValue,
                    CountryCode = attributeSet.getAttribute("c")?.StringValue
                },

                SamAccountType = int.Parse(attributeSet.getAttribute("sAMAccountType")?.StringValue ?? "0"),
            };

            return ldapUser;
        }

        /// <summary>
        /// Create Entry From Attributes
        /// </summary>
        /// <param name="distinguishedName"></param>
        /// <param name="attributeSet"></param>
        /// <returns></returns>
        protected virtual LdapEntry CreateEntryFromAttributes(string distinguishedName, LdapAttributeSet attributeSet)
        {
            return new LdapEntry
            {
                ObjectSid = attributeSet.getAttribute("objectSid")?.StringValue,
                ObjectGuid = attributeSet.getAttribute("objectGUID")?.StringValue,
                ObjectCategory = attributeSet.getAttribute("objectCategory")?.StringValue,
                ObjectClass = attributeSet.getAttribute("objectClass")?.StringValue,
                CommonName = attributeSet.getAttribute("cn")?.StringValue,
                Name = attributeSet.getAttribute("name")?.StringValue,
                DistinguishedName = attributeSet.getAttribute("distinguishedName")?.StringValue ?? distinguishedName,
                SamAccountName = attributeSet.getAttribute("sAMAccountName")?.StringValue,
                SamAccountType = int.Parse(attributeSet.getAttribute("sAMAccountType")?.StringValue ?? "0"),
            };
        }

        /// <summary>
        /// Get Domain Sid
        /// </summary>
        /// <returns></returns>
        protected virtual SecurityIdentifier GetDomainSid()
        {
            var administratorAcount = new NTAccount(_ldapSettings.DomainName, "administrator");
            var administratorSId = (SecurityIdentifier)administratorAcount.Translate(typeof(SecurityIdentifier));
            return administratorSId.AccountDomainSid;
        }
    }
}