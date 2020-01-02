using System.Collections.Generic;
using GR.Backup.Abstractions.Models;
using GR.Cache.Abstractions;
using GR.Core;
using GR.Email.Abstractions.Models.EmailViewModels;
using GR.Localization.Abstractions.ViewModels.LocalizationViewModels;

namespace GR.WebApplication.Models
{
    public sealed class AppSettingsModel
    {
        public RootObject RootObjects { get; set; }

        public sealed class RootObject
        {
            /// <summary>
            /// Is Configured
            /// </summary>
            public bool IsConfigured { get; set; } = false;

            /// <summary>
            /// System config data
            /// </summary>
            public SystemConfig SystemConfig { get; set; } = new SystemConfig();

            /// <summary>
            /// Connection Strings
            /// </summary>
            public ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
            /// <summary>
            /// Logging
            /// </summary>
            public Logging Logging { get; set; } = new Logging();
            /// <summary>
            /// Health Check
            /// </summary>
            public HealthCheck HealthCheck { get; set; } = new HealthCheck();
            /// <summary>
            /// Localization Config
            /// </summary>
            public LocalizationConfigModel LocalizationConfig { get; set; } = new LocalizationConfigModel();
            /// <summary>
            /// LdapSettings
            /// </summary>
            public LdapSettings LdapSettings { get; set; } = new LdapSettings();
            /// <summary>
            /// WebClients
            /// </summary>
            public Dictionary<string, Dictionary<string, string>> WebClients { get; set; } = new Dictionary<string, Dictionary<string, string>>();

            /// <summary>
            /// Backup settings
            /// </summary>
            public BackupSettings BackupSettings { get; set; } = new BackupSettings();

            /// <summary>
            /// Email settings
            /// </summary>
            public EmailSettingsViewModel EmailSettings { get; set; } = new EmailSettingsViewModel();

            /// <summary>
            /// Redis connection configuration
            /// </summary>
            public RedisConnectionConfig RedisConnection { get; set; } = new RedisConnectionConfig();
        }

        public sealed class ConnectionStrings
        {
            public string Provider { get; set; }
            public string ConnectionString { get; set; }
        }

        public sealed class LogLevel
        {
            public string Default { get; set; }
        }

        public sealed class Logging
        {
            public bool IncludeScopes { get; set; }
            public LogLevel LogLevel { get; set; }
        }

        public sealed class HealthCheck
        {
            public int Timeout { get; set; }
            public string Path { get; set; }
        }

        public sealed class Credentials
        {
            public string DomainUserName { get; set; }
            public string Password { get; set; }
        }

        public sealed class LdapSettings
        {
            public string ServerName { get; set; }
            public int ServerPort { get; set; }
            public bool UseSSL { get; set; }
            public Credentials Credentials { get; set; } = new Credentials();
            public string SearchBase { get; set; }
            public string ContainerName { get; set; }
            public string DomainName { get; set; }
            public string DomainDistinguishedName { get; set; }
        }
    }
}
