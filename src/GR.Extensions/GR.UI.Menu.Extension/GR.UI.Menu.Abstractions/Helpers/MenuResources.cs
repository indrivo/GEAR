using System;

namespace GR.UI.Menu.Abstractions.Helpers
{
    public static class MenuResources
    {
        public static Guid AppMenuId = Guid.Parse("46EACBA3-D515-47B0-9BA7-5391CE1D26B1".ToLower());

        public static class MenuItems
        {
            public const string None = "#";
            public static Guid AdministrationItem = Guid.Parse("6023b0a4-6dd5-4c95-90c7-616571200a5b");
            public static Guid AppearanceMenuItem = Guid.Parse("004e7722-fcb2-4aa4-b284-25c860b84cd6");
            public static Guid StructureMenuItem = Guid.Parse("2023e10e-7c08-489d-afa6-7cac1b1e7939");
            public static Guid HomeItem = Guid.Parse("755cdc9b-35f1-4943-ae36-783a3faf758d");
            public static Guid AppsItem = Guid.Parse("95318b7d-7041-4acf-80d0-845a250f0e78");
            public static Guid ConfigurationItem = Guid.Parse("838bc0c0-eb37-4ddf-97e4-bbf09fb090a7");
            public static Guid MenuItem = Guid.Parse("9547d4cb-9148-4ca3-8a29-133705e455dc");
        }
    }
}
