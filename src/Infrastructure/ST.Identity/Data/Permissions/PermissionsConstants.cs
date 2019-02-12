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
        }

        public static class HrmPermissions
        {
            #region Department

            public const string HrmDepartmentCreate = "Hrm_DepartmentCreate";
            public const string HrmDepartmentUpdate = "Hrm_DepartmentUpdate";
            public const string HrmDepartmentDelete = "Hrm_DepartmentDelete";
            public const string HrmDepartmentRead   = "Hrm_DepartmentRead";

            #endregion

            #region Job

            public const string HrmJobCreate = "Hrm_JobCreate";
            public const string HrmJobUpdate = "Hrm_JobUpdate";
            public const string HrmJobDelete = "Hrm_JobDelete";
            public const string HrmJobRead   = "Hrm_JobRead";

            #endregion

            #region Candidate

            public const string HrmCandidateCreate = "Hrm_CandidateCreate";
            public const string HrmCandidateUpdate = "Hrm_CandidateUpdate";
            public const string HrmCandidateDelete = "Hrm_CandidateDelete";
            public const string HrmCandidateRead   = "Hrm_CandidateRead";
            public const string HrmAddToEmployees  = "Hrm_AddToEmployees";

            #endregion

            #region Interview

            public const string HrmInterviewCreate = "Hrm_InterviewCreate";
            public const string HrmInterviewUpdate = "Hrm_InterviewUpdate";
            public const string HrmInterviewDelete = "Hrm_InterviewDelete";
            public const string HrmInterviewRead   = "Hrm_InterviewRead";

            #endregion

            #region Employee

            public const string HrmEmployeeCreate = "Hrm_EmployeeCreate";
            public const string HrmEmployeeUpdate = "Hrm_EmployeeUpdate";
            public const string HrmEmployeeDelete = "Hrm_EmployeeDelete";
            public const string HrmEmployeeRead   = "Hrm_EmployeeRead";

            #endregion

            #region Tasks

            public const string HrmTaskCreate = "Hrm_TaskCreate";
            public const string HrmTaskUpdate = "Hrm_TaskUpdate";
            public const string HrmTaskDelete = "Hrm_TaskDelete";
            public const string HrmTaskRead = "Hrm_TaskRead";

            #endregion

            #region Leaves

            public const string HrmLeaveCreate = "Hrm_LeaveCreate";
            public const string HrmLeaveUpdate = "Hrm_LeaveUpdate";
            public const string HrmLeaveDelete = "Hrm_LeaveDelete";
            public const string HrmApproveLeave = "Hrm_ApproveLeave";
            public const string HrmLeaveRead = "Hrm_LeaveRead";

            #endregion

            #region Reports

            public const string HrmReportCreate = "Hrm_ReportCreate";
            public const string HrmReportDelete = "Hrm_ReportDelete";
            public const string HrmReportRead = "Hrm_ReportRead";

            #endregion
        }

        public static class PmPermissions
        {
        }

        public static class BscPermissions
        {
        }

        public static class CrmPermissions
        {
            #region Pipeline

            public const string CrmPipelineCreate = "Crm_PipelineCreate";
            public const string CrmPipelineRead = "Crm_PipelineRead";
            public const string CrmPipelineUpdate = "Crm_PipelineUpdate";
            public const string CrmPipelineDelete = "Crm_PipelineDelete";

            #endregion


            #region Stages

            public const string CrmStageCreate = "Crm_StageCreate";
            public const string CrmStageRead = "Crm_StageRead";
            public const string CrmStageUpdate = "Crm_StageUpdate";
            public const string CrmStageDelete = "Crm_StageDelete";

            #endregion


            #region Deals

            public const string CrmDealCreate = "Crm_DealCreate";
            public const string CrmDealRead = "Crm_DealRead";
            public const string CrmDealUpdate = "Crm_DealUpdate";
            public const string CrmDealDelete = "Crm_DealDelete";

            #endregion


            #region Activities

            public const string CrmActivitiesCreate = "Crm_ActivitiesCreate";
            public const string CrmActivitiesRead = "Crm_ActivitiesRead";
            public const string CrmActivitiesUpdate = "Crm_ActivitiesUpdate";
            public const string CrmActivitiesDelete = "Crm_ActivitiesDelete";
            public const string CrmActivitiesMarkAsComplete = "Crm_ActivitiesMarkAsComplete";

            #endregion


            #region ActivityTypes

            public const string CrmActivityTypeCreate = "Crm_ActivityTypeCreate";
            public const string CrmActivityTypeRead = "Crm_ActivityTypeRead";
            public const string CrmActivityTypeUpdate = "Crm_ActivityTypeUpdate";
            public const string CrmActivityTypeDelete = "Crm_ActivityTypeDelete";


            #endregion


            #region ClientOrganization

            public const string CrmClientOrganizationCreate = "Crm_ClientOrganizationCreate";
            public const string CrmClientOrganizationRead = "Crm_ClientOrganizationRead";
            public const string CrmClientOrganizationUpdate = "Crm_ClientOrganizationUpdate";
            public const string CrmClientOrganizationDelete = "Crm_ClientOrganizationDelete";

            #endregion


            #region ClientContact

            public const string CrmClientContactCreate = "Crm_ClientContactCreate";
            public const string CrmClientContactRead = "Crm_ClientContactRead";
            public const string CrmClientContactUpdate = "Crm_ClientContactUpdate";
            public const string CrmClientContactDelete = "Crm_ClientContactDelete";

            #endregion


            #region DealSpecific Types

            public const string CrmDealServicesTypeAdmin = "Crm_DealServicesTypeAdmin";

            public const string CrmDealServiceTypeCreate = "Crm_DealServiceTypeCreate";
            public const string CrmDealServiceTypeRead = "Crm_DealServiceTypeRead";
            public const string CrmDealServiceTypeUpdate = "Crm_DealServiceTypeUpdate";
            public const string CrmDealServiceTypeDelete = "Crm_DealServiceTypeDelete";


            public const string CrmDealProductTypeCreate = "Crm_DealProductTypeCreate";
            public const string CrmDealProductTypeRead = "Crm_DealProductTypeRead";
            public const string CrmDealProductTypeUpdate = "Crm_DealProductTypeUpdate";
            public const string CrmDealProductTypeDelete = "Crm_DealProductTypeDelete";


            public const string CrmDealSolutionTypeCreate = "Crm_DealSolutionTypeCreate";
            public const string CrmDealSolutionTypeRead = "Crm_DealSolutionTypeRead";
            public const string CrmDealSolutionTypeUpdate = "Crm_DealSolutionTypeUpdate";
            public const string CrmDealSolutionTypeDelete = "Crm_DealSolutionTypeDelete";


            public const string CrmDealTechnologyTypeCreate = "Crm_DealTechnologyTypeCreate";
            public const string CrmDealTechnologyTypeRead = "Crm_DealTechnologyTypeRead";
            public const string CrmDealTechnologyTypeUpdate = "Crm_DealTechnologyTypeUpdate";
            public const string CrmDealTechnologyTypeDelete = "Crm_DealTechnologyTypeDelete";

            #endregion


            #region Dashboard

            public const string CrmDashboardCreate = "Crm_DashboardCreate";
            public const string CrmDashboardRead = "Crm_DashboardRead";
            public const string CrmDashboardUpdate = "Crm_DashboardUpdate";
            public const string CrmDashboardDelete = "Crm_DashboardDelete";

            #endregion

            #region Reports

            public const string CrmReportCreate = "Crm_ReportCreate";
            public const string CrmReportRead = "Crm_ReportRead";
            public const string CrmReportUpdate = "Crm_ReportUpdate";
            public const string CrmReportDelete = "Crm_ReportDelete";

            #endregion

            #region Currency

            public const string CrmCurrencyCreate = "Crm_CurrencyCreate";
            public const string CrmCurrencyRead = "Crm_CurrencyRead";
            public const string CrmCurrencyUpdate = "Crm_CurrencyUpdate";
            public const string CrmCurrencyDelete = "Crm_CurrencyDelete";


            #endregion

            #region DealSource

            public const string CrmDealSourceCreate = "Crm_DealSourceCreate";
            public const string CrmDealSourceRead = "Crm_DealSourceRead";
            public const string CrmDealSourceUpdate = "Crm_DealSourceUpdate";
            public const string CrmDealSourceDelete = "Crm_DealSourceDelete";


            #endregion

        }


        public static IEnumerable<string> PermissionsList(ClientName client = ClientName.All)
        {
            var permissions = new List<string>();

            switch (client)
            {
                case ClientName.Core:
                    {
                        var corePermissions = typeof(CorePermissions).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        permissions.AddRange(corePermissions.Select(_ => _.GetValue(null).ToString()).ToList());
                    }
                    break;
                case ClientName.Bsc:
                    {
                        var bscPermissions = typeof(BscPermissions).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        permissions.AddRange(bscPermissions.Select(_ => _.GetValue(null).ToString()));
                    }
                    break;
                case ClientName.Crm:
                    {
                        var crmPermissions = typeof(CrmPermissions).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        permissions.AddRange(crmPermissions.Select(_ => _.GetValue(null).ToString()));
                    }
                    break;
                case ClientName.Hrm:
                    {
                        var hrmPermissions = typeof(HrmPermissions).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        permissions.AddRange(hrmPermissions.Select(_ => _.GetValue(null).ToString()));
                    }
                    break;
                case ClientName.Pm:
                    {
                        var pmPermissions = typeof(PmPermissions).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        permissions.AddRange(pmPermissions.Select(_ => _.GetValue(null).ToString()));
                    }
                    break;
                case ClientName.All:
                    {
                        var corePermissions = typeof(CorePermissions).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        permissions.AddRange(corePermissions.Select(_ => _.GetValue(null).ToString()).ToList());
                        var hrmPermissions = typeof(HrmPermissions).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        permissions.AddRange(hrmPermissions.Select(_ => _.GetValue(null).ToString()));
                        var crmPermissions = typeof(CrmPermissions).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        permissions.AddRange(crmPermissions.Select(_ => _.GetValue(null).ToString()));
                        var bscPermissions = typeof(BscPermissions).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        permissions.AddRange(bscPermissions.Select(_ => _.GetValue(null).ToString()));
                        var pmPermissions = typeof(PmPermissions).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        permissions.AddRange(pmPermissions.Select(_ => _.GetValue(null).ToString()));
                    }
                    break;
            }

            return permissions;
        }

        public enum ClientName
        {
            Core,
            Hrm,
            Crm,
            Bsc,
            Pm,
            All
        }
    }
}