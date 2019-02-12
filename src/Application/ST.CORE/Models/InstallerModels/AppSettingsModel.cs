using System.Collections.Generic;

namespace ST.CORE.Models.InstallerModels
{
    public class AppSettingsModel
    {

	    public RootObject RootObjects { get; set; }

	    public class ConnectionStrings
	    {
		    public string DefaultConnection { get; set; }
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

	    public class RootObject
	    {
		    public ConnectionStrings ConnectionStrings { get; set; }
		    public Logging Logging { get; set; }
		    public HealthCheck HealthCheck { get; set; }
		    public LocalizationConfig LocalizationConfig { get; set; }
		    public bool IsConfigurated { get; set; }
		}
	}
}
