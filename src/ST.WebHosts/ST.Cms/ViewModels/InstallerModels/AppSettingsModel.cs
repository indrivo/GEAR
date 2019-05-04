using System.Collections.Generic;

namespace ST.Cms.ViewModels.InstallerModels
{
	public class AppSettingsModel
	{

		public RootObject RootObjects { get; set; }

		public class PostgreSQL
		{
			public bool UsePostgreSQL { get; set; }
			public string ConnectionString { get; set; }
		}

		public class ConnectionStrings
		{
			public string MSSQLConnection { get; set; }
			public PostgreSQL PostgreSQL { get; set; }
		}

		public class LogLevel
		{
			public string Default { get; set; }
		}

		public class Logging
		{
			public bool IncludeScopes { get; set; }
			public LogLevel LogLevel { get; set; }
		}

		public class HealthCheck
		{
			public int Timeout { get; set; }
			public string Path { get; set; }
		}

		public class Language
		{
			public string Identifier { get; set; }
			public string Name { get; set; }
		}

		public class LocalizationConfig
		{
			public List<Language> Languages { get; set; }
			public string Path { get; set; }
			public string SessionStoreKeyName { get; set; }
			public string DefaultLanguage { get; set; }
		}

		public class Credentials
		{
			public string DomainUserName { get; set; }
			public string Password { get; set; }
		}

		public class LdapSettings
		{
			public string ServerName { get; set; }
			public int ServerPort { get; set; }
			public bool UseSSL { get; set; }
			public Credentials Credentials { get; set; }
			public string SearchBase { get; set; }
			public string ContainerName { get; set; }
			public string DomainName { get; set; }
			public string DomainDistinguishedName { get; set; }
		}

		public class RootObject
		{
			/// <summary>
			/// Connection Strings
			/// </summary>
			public ConnectionStrings ConnectionStrings { get; set; }
			/// <summary>
			/// Logging
			/// </summary>
			public Logging Logging { get; set; }
			/// <summary>
			/// Health Check
			/// </summary>
			public HealthCheck HealthCheck { get; set; }
			/// <summary>
			/// Localization Config
			/// </summary>
			public LocalizationConfig LocalizationConfig { get; set; }
			/// <summary>
			/// Is Configured
			/// </summary>
			public bool IsConfigured { get; set; }
			/// <summary>
			/// LdapSettings
			/// </summary>
			public LdapSettings LdapSettings { get; set; }
			/// <summary>
			/// WebClients
			/// </summary>
			public Dictionary<string, Dictionary<string, string>> WebClients { get; set; }
		}
	}
}
