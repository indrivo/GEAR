using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GR.Identity.Data.Permissions
{
    public static class PermissionsConstants
    {
        public static class CorePermissions
        {
            #region Roles

            public const string BpmCreateRole = "Core_CreateRole";
            public const string BpmEditRole = "Core_EditRole";
            public const string BpmReadRole = "Core_ReadRole";
            public const string BpmDeleteRole = "Core_DeleteRole";

            #endregion Roles

            #region Processes

            

            #endregion Processes
        }

        public static IEnumerable<string> PermissionsList(ClientName client = ClientName.All)
        {
            var permissions = new List<string>();

            switch (client)
            {
                case ClientName.Core:// specific core
                    {
                        var corePermissions = typeof(CorePermissions).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        permissions.AddRange(corePermissions.Select(_ => _.GetValue(null).ToString()).ToList());
                    }
                    break;

                case ClientName.All:
                    {
                        var corePermissions = typeof(CorePermissions).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        permissions.AddRange(corePermissions.Select(_ => _.GetValue(null).ToString()).ToList());
                    }
                    break;
            }

            return permissions;
        }

        public enum ClientName
        {
            Core,
            All
        }
    }
}