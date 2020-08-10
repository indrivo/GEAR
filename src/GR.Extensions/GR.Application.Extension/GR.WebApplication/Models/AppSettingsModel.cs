using GR.Core;

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
    }
}
