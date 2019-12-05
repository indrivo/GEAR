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

            #endregion

            #region Groups

            public const string BpmGroupCreate = "Core_GroupCreate";
            public const string BpmGroupRead = "Core_GroupRead";
            public const string BpmGroupUpdate = "Core_GroupUpdate";
            public const string BpmGroupDelete = "Core_GroupDelete";

            #endregion

            #region Users

            public const string BpmUserCreate = "Core_UserCreate";
            public const string BpmUserRead = "Core_UserRead";
            public const string BpmUserUpdate = "Core_UserUpdate";
            public const string BpmUserDelete = "Core_UserDelete";

            #endregion

            #region Profiles

            public const string BpmProfileCreate = "Core_ProfileCreate";
            public const string BpmProfileRead = "Core_ProfileRead";
            public const string BpmProfileUpdate = "Core_ProfileUpdate";
            public const string BpmProfileDelete = "Core_ProfileDelete";

            #endregion

            #region Processes

            public const string BpmProcessCreate = "Core_ProcessCreate";
            public const string BpmProcessUpdate = "Core_ProcessUpdate";
            public const string BpmProcessRead = "Core_ProcessRead";
            public const string BpmProcessDelete = "Core_ProcessDelete";

            #endregion

            #region Entity

            public const string BpmEntityCreate = "Core_EntityCreate";
            public const string BpmEntityUpdate = "Core_EntityUpdate";
            public const string BpmEntityRead = "Core_EntityRead";
            public const string BpmEntityDelete = "Core_EntityDelete";

            #endregion

            #region Forms

            public const string BpmFormCreate = "Core_FormCreate";
            public const string BpmFormUpdate = "Core_FormUpdate";
            public const string BpmFormRead = "Core_FormRead";
            public const string BpmFormDelete = "Core_FormDelete";

            #endregion

            #region Tables

            public const string BpmTableCreate = "Core_TableCreate";
            public const string BpmTableUpdate = "Core_TableUpdate";
            public const string BpmTableRead = "Core_TableRead";
            public const string BpmTableDelete = "Core_TableDelete";

            #endregion
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