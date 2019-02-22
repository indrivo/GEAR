using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ST.Identity.Data.Permissions
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

            /****ISODMS Permissions*****/

            #region Requirement

            public const string BpmRequirementCreate = "Core_RequirementCreate";
            public const string BpmRequirementUpdate = "Core_RequirementUpdate";
            public const string BpmRequirementRead = "Core_RequirementRead";
            public const string BpmRequirementDelete = "Core_RequirementDelete";

            #endregion

            #region Objective

            public const string BpmObjectiveCreate = "Core_ObjectiveCreate";
            public const string BpmObjectiveUpdate = "Core_ObjectiveUpdate";
            public const string BpmObjectiveRead = "Core_ObjectiveRead";
            public const string BpmObjectiveDelete = "Core_ObjectiveDelete";

            #endregion

            #region Asset

            public const string BpmAssetCreate = "Core_AssetCreate";
            public const string BpmAssetUpdate = "Core_AssetUpdate";
            public const string BpmAssetRead = "Core_AssetRead";
            public const string BpmAssetDelete = "Core_AssetDelete";

            #endregion

            #region Risk

            public const string BpmRiskCreate = "Core_RiskCreate";
            public const string BpmRiskUpdate = "Core_RiskUpdate";
            public const string BpmRiskRead = "Core_RiskRead";
            public const string BpmRiskDelete = "Core_RiskDelete";
            public const string BpmRiskAssetUpdate = "Core_RiskAssetUpdate";
            public const string BpmRiskAssetRead = "Core_RiskAssetRead";

            #endregion

            #region Check

            public const string BpmCheckCreate = "Core_CheckCreate";
            public const string BpmCheckUpdate = "Core_CheckUpdate";
            public const string BpmCheckRead = "Core_CheckRead";
            public const string BpmCheckDelete = "Core_CheckDelete";

            #endregion

            #region KPI

            public const string BpmKPICreate = "Core_KPICreate";
            public const string BpmKPIUpdate = "Core_KPIUpdate";
            public const string BpmKPIRead = "Core_KPIRead";
            public const string BpmKPIDelete = "Core_KPIDelete";

            #endregion

            #region InternalAudit

            public const string BpmInternalAuditCreate = "Core_InternalAuditCreate";
            public const string BpmInternalAuditUpdate = "Core_InternalAuditUpdate";
            public const string BpmInternalAuditRead = "Core_InternalAuditRead";
            public const string BpmInternalAuditDelete = "Core_InternalAuditDelete";

            #endregion

            #region ExternalAudit

            public const string BpmExternalAuditCreate = "Core_ExternalAuditCreate";
            public const string BpmExternalAuditUpdate = "Core_ExternalAuditUpdate";
            public const string BpmExternalAuditRead = "Core_ExternalAuditRead";
            public const string BpmExternalAuditDelete = "Core_ExternalAuditDelete";

            #endregion

            #region ManagementAnalysis

            public const string BpmManagementAnalysisCreate = "Core_ManagementAnalysisCreate";
            public const string BpmManagementAnalysisUpdate = "Core_ManagementAnalysisUpdate";
            public const string BpmManagementAnalysisRead = "Core_ManagementAnalysisRead";
            public const string BpmManagementAnalysisDelete = "Core_ManagementAnalysisDelete";

            #endregion

            #region ActionPlan

            public const string BpmActionPlanCreate = "Core_ActionPlanCreate";
            public const string BpmActionPlanUpdate = "Core_ActionPlanUpdate";
            public const string BpmActionPlanRead = "Core_ActionPlanRead";
            public const string BpmActionPlanDelete = "Core_ActionPlanDelete";

            #endregion

            #region Documents

            public const string BpmDocumentsCreate = "Core_DocumentsCreate";
            public const string BpmDocumentsUpdate = "Core_DocumentsUpdate";
            public const string BpmDocumentsRead = "Core_DocumentsRead";
            public const string BpmDocumentsDelete = "Core_DocumentsDelete";

            #endregion

            #region Range

            public const string BpmRangeCreate = "Core_RangeCreate";
            public const string BpmRangeUpdate = "Core_RangeUpdate";
            public const string BpmRangeRead = "Core_RangeRead";
            public const string BpmRangeDelete = "Core_RangeDelete";

            #endregion

            #region InternalAuditPlan

            public const string BpmInternalAuditPlanCreate = "Core_InternalAuditPlanCreate";
            public const string BpmInternalAuditPlanUpdate = "Core_InternalAuditPlanUpdate";
            public const string BpmInternalAuditPlanRead = "Core_InternalAuditPlanRead";
            public const string BpmInternalAuditPlanDelete = "Core_InternalAuditPlanDelete";

            #endregion

            #region ExternalAuditPlan

            public const string BpmExternalAuditPlanCreate = "Core_ExternalAuditPlanCreate";
            public const string BpmExternalAuditPlanUpdate = "Core_ExternalAuditPlanUpdate";
            public const string BpmExternalAuditPlanRead = "Core_ExternalAuditPlanRead";
            public const string BpmExternalAuditPlanDelete = "Core_ExternalAuditPlanDelete";

            #endregion

            #region Reports

            public const string BpmReportsCreate = "Core_ReportsCreate";
            public const string BpmReportsUpdate = "Core_ReportsUpdate";
            public const string BpmReportsRead = "Core_ReportsRead";
            public const string BpmReportsDelete = "Core_ReportsDelete";

            #endregion

            #region DashBoard

            public const string BpmDashBoardCreate = "Core_DashBoardCreate";
            public const string BpmDashBoardUpdate = "Core_DashBoardUpdate";
            public const string BpmDashBoardRead = "Core_DashBoardRead";
            public const string BpmDashBoardDelete = "Core_DashBoardDelete";

            #endregion

            #region Administration

            public const string BpmAdministrationCreate = "Core_AdministrationCreate";
            public const string BpmAdministrationUpdate = "Core_AdministrationUpdate";
            public const string BpmAdministrationRead = "Core_AdministrationRead";
            public const string BpmAdministrationDelete = "Core_AdministrationDelete";

            #endregion     
       
            /***************************/
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