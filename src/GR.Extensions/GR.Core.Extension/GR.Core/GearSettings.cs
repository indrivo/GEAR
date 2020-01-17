using System;
// ReSharper disable InconsistentNaming

namespace GR.Core
{
    public static class GearSettings
    {
        /// <summary>
        /// Default tenant id
        /// </summary>
        public static Guid TenantId = Guid.Parse("d11eeb3d-9545-4f1a-a199-632257326765");

        /// <summary>
        /// Default language
        /// </summary>
        public const string DEFAULT_LANGUAGE = "English";

        /// <summary>
        /// Entity default schema
        /// </summary>
        public const string DEFAULT_ENTITY_SCHEMA = "system";

        /// <summary>
        /// Access denied message
        /// </summary>
        public const string ACCESS_DENIED_MESSAGE = "Access denied";

        public struct Date
        {
            public const string DateFormat = "dd'.'MM'.'yyyy";
            public const string DateFormatWithTime = "dd'.'MM'.'yyyy hh:mm:ss tt";
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
