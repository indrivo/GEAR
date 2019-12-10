namespace GR.MultiTenant.Abstractions.Helpers
{
    public struct MultiTenantResources
    {
        public struct Translations
        {
            public const string TENANT_LOGO = "system_tenant_logo";
        }

        public struct EmbeddedResources
        {
            public const string COMPANY_IMAGE = "Static/Embedded Resources/company.png";
        }

        public struct Exceptions
        {
            public const string E_MULTI_TENANT_COMPANY_IMAGE_NULL = "Default company image not found";
        }

        public struct RegularExpressions
        {
            public const string TENANT_MACHINE_NAME = " *[+=^'$(),.!\\s\\~#%&*{}/:<>?|\"-]+ *";
        }

        public struct Roles
        {
            public const string COMPANY_ADMINISTRATOR = "Company Administrator";
        }
    }
}
