namespace GR.Identity.Abstractions.Helpers
{
    public static class Resources
    {
        public static class Translations
        {
            public const string CONTACT_NAME = "user_contact_name";
            public const string PHONE = "system_phone";
            public const string ADDRESS_LINE1 = "user_address_line_1";
            public const string ADRESS_LINE2 = "user_address_line_2";
            public const string CITY = "system_city";
            public const string COUNTRY = "system_country";
            public const string ZIP_CODE = "user_zip_code";
            public const string IS_DEFAULT = "system_is_default";
        }

        public static class ValidationMessages
        {
            public const string PASSWORD_STRING_LENGTH = "The {0} must be at least {2} and at max {1} characters long.";
            public const string PASSWORD_COMPLEXITY_MESSAGE = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)";
        }

        public static class RegularExpressions
        {
            /// <summary>
            /// Password
            /// </summary>
            public const string PASSWORD = "^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$";
        }
    }
}