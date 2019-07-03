using System;

namespace ST.Identity.LdapAuth.Abstractions.Exceptions
{
    public class LdapSettingsNotFoundException : Exception
    {
        public LdapSettingsNotFoundException() : base("In appsettings are missing ldap settings, use documentation on usage for this module")
        {

        }
    }
}
