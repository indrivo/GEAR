using System;

namespace ST.Core
{
    public static class Settings
    {
        public static string DefaultLanguage = "English";

        public static Guid TenantId = Guid.Parse("d11eeb3d-9545-4f1a-a199-632257326765");

        public const string SuperAdmin = "Administrator";

        public static string DefaultEntitySchema = "system";

        public struct Tables
        {
            public const string CustomTable = "CustomTableName";
        }

        public static class Date
        {
            public static string DateFormat = "dd'.'MM'.'yyyy";
        }
    }

    public sealed class SystemConfig
    {
        /// <summary>
        /// This property value is used for cookie name and redis reserved key
        /// </summary>
        public string MachineIdentifier { get; set; }
    }
}
