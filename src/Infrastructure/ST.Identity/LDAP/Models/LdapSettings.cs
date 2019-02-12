using System;
using System.Collections.Generic;
using System.Text;

namespace ST.Identity.LDAP.Models
{
    public class LdapSettings
    {
        public string ServerName { get; set; }

        public int ServerPort { get; set; }

        public bool UseSSL { get; set; }

        public string SearchBase { get; set; }

        public string ContainerName { get; set; }

        public string DomainName { get; set; }

        public string DomainDistinguishedName { get; set; }

        public LdapCredentials Credentials { get; set; }
    }
}
