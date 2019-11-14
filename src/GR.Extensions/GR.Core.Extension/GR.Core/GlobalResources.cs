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
            /// Anonimous User role
            /// </summary>
            public const string ANONIMOUS_USER = "Anonymous User";

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
        }
    }
}
