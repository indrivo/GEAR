using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using GR.Core.Helpers;
using GR.Core.Razor.Enums;
using GR.Core.Razor.TagHelpers.Drawers;
using GR.Core.Razor.TagHelpers.TagHelperViewModels.ListTagHelperViewModels;
using GR.Core.Razor.TagHelpersStructures;

namespace GR.MultiTenant.Razor.Helpers
{
    public class MultiTenantListSettings
    {
        /// <summary>
        /// Localizer
        /// </summary>
        private readonly IStringLocalizer _localizer;

        public MultiTenantListSettings()
        {
            _localizer = IoC.Resolve<IStringLocalizer>();
        }

        public ListTagHelperModel GetCompanyUserListSettings()
        {
            var model = new ListTagHelperModel
            {
                Title = _localizer["system_multi_tenant_manage_company_users"],
                SubTitle = "",
                ListIdentifier = "manageCompanyUsers",
                Api = new ListApiConfigurationViewModel
                {
                    Url = "/CompanyManage/LoadPageItems"
                },
                StyleAttributes = new List<InlineStyleAttribute>
                {
                    new InlineStyleAttribute("width", "100%")
                },
                ListActionDrawer = new DropDownListActionDrawer(),
                RenderColumns = new List<ListRenderColumn>
                {
                    new ListRenderColumn(_localizer["name"], "userName"),
                    new ListRenderColumn(_localizer["system_user_online"], "isOnline")
                    {
                        HasTemplate = true,
                        Template = @"{
                            data: null,
                            render: function(data, type, row, meta){
                                const color = row.isOnline ? 'green' : 'red';
                                return `<div class='color-indicator' style='background-color:${color}'></div>`;
                            }
                        },"
                    },
                    new ListRenderColumn(_localizer["is_deleted"], "isDeleted")
                    {
                        BodySystemTemplate = RenderCellBodySystemTemplate.Boolean
                    },
                    new ListRenderColumn(_localizer["roles"], "roles"),
                    new ListRenderColumn(_localizer["email"], "email"),
                    new ListRenderColumn(_localizer["system_email_confirmed"], "emailConfirmed")
                    {
                        HasTemplate = true,
                        Template = @"{
                                        data: null, 
                                        render: function(data, type, row, meta) {
                                            if (row.emailConfirmed) return `<span class='badge badge-success'>${window.translate('system_email_confirmed')}</span>`
                                            return `<span class='badge badge-danger'>${window.translate('system_email_not_confirmed')}</span>`;
                                        }},"
                    },
                    new ListRenderColumn(_localizer["created"], "created"),
                    new ListRenderColumn(_localizer["author"], "author"),
                    new ListRenderColumn(_localizer["changed"], "changed"),
                    new ListRenderColumn(_localizer["modified_by"], "modifiedBy")
                },
                HeadButtons = new List<UrlTagHelperViewModel>
                {
                    new UrlTagHelperViewModel
                    {
                        AspAction = null,
                        AspController = "CompanyManage",
                        ButtonName = $"<i class='mdi mdi-account-multiple-plus mr-2' aria-hidden='true'></i> {_localizer["system_invite_user"]}",
                        Description = null,
                        BootstrapButton = BootstrapButton.Success,
                        HtmlAttributes = new List<HtmlAttribute>
                        {
                            new HtmlAttribute("id", "inviteBtn")
                        }
                    }
                },
                HasActions = true,
                ListActions = new List<ListActionViewModel>
                {
                    new ListActionViewModel
                    {
                        HasIcon = false,
                        Name = _localizer["system_user_change_password"],
                        Url = "/Users/ChangeUserPassword",
                        ActionParameters = new List<ActionParameter>
                        {
                            new ActionParameter("userId", "id"),
                            new ActionParameter("callBackUrl", "/CompanyManage")
                            {
                                IsCustomValue = true
                            }
                        },
                        ButtonType = BootstrapButton.Warning
                    },
                    new ListActionViewModel
                    {
                        HasIcon = false,
                        Name = _localizer["system_change_roles"],
                        IsJsEvent = true,
                        ButtonEvent = new JsActionButtonEvent
                        {
                            JsEvent = JsEvent.OnClick,
                            JsEventHandler = "changeRoles('${row.id}')"
                        },
                        ButtonType = BootstrapButton.Info
                    },
                    new ListActionViewModel
                    {
                        HasIcon = false,
                        Name = _localizer["delete"],
                        Url = "/CompanyManage/DeleteUser",
                        ActionParameters = new List<ActionParameter>
                        {
                            new ActionParameter("userId", "id")
                        },
                        ButtonType = BootstrapButton.Danger
                    }
                },
                Documentation = _localizer["system_multi_tenant_company_users_doc"]
            };
            return model;
        }
    }
}