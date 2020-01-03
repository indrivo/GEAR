using System;

namespace GR.UI.Menu.Abstractions.Helpers
{
    public static class MenuResources
    {
        public static Guid AppMenuId = Guid.Parse("46EACBA3-D515-47B0-9BA7-5391CE1D26B1".ToLower());

        public static class MenuItems
        {
            public static Guid AdministrationItem = Guid.Parse("6023b0a4-6dd5-4c95-90c7-616571200a5b");
            public static Guid HomeItem = Guid.Parse("755cdc9b-35f1-4943-ae36-783a3faf758d");
            public static Guid AppsItem = Guid.Parse("95318b7d-7041-4acf-80d0-845a250f0e78");
        }
    }
}
