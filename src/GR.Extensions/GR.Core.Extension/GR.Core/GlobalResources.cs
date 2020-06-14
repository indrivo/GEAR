// ReSharper disable InconsistentNaming
namespace GR.Core
{
    public struct GlobalResources
    {
        public struct Roles
        {
            /// <summary>
            /// Super admin role
            /// </summary>
            public const string ADMINISTRATOR = "Administrator";

            /// <summary>
            /// Anonymous User role
            /// </summary>
            public const string ANONYMOUS_USER = "Anonymous User";

            /// <summary>
            /// User role
            /// </summary>
            public const string USER = "User";
        }

        public struct RegularExpressions
        {
            /// <summary>
            /// Is guid format
            /// </summary>
            public const string GUID = @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$";

            /// <summary>
            /// Is email format
            /// </summary>
            public const string EMAIL = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            
            /// <summary>
            /// Is credit card format
            /// </summary>
            public const string CREDIT_CARD = @"^(?:4[0-9]{12}(?:[0-9]{3})?|[25][1-7][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$";
        }

        public struct Environments
        {
            public const string RELEASE = "Release";
            public const string DEVELOPMENT = "Development";
            public const string STAGE = "Stage";
        }

        public struct Paths
        {
            public const string CertificatesPath = "Certificates";
            public const string EmbeddedResourcesPath = "Static/EmbeddedResources";
        }
    }
}
