using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using ST.Core.Helpers;
using ST.Core.Razor.TagHelpers.TagHelperViewModels.ListTagHelperViewModels;
using ST.Core.Razor.TagHelpersStructures;

namespace ST.MultiTenant.Razor.Helpers
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
                Title = "Manage my company users",
                SubTitle = "This is the list of your company users",
                ListIdentifier = "manageCompanyUsers",
                Api = new ListApiConfigurationViewModel
                {
                    Url = "/CompanyManage/LoadPageItems"
                },
                StyleAttributes = new List<InlineStyleAttribute>
                {
                    new InlineStyleAttribute("width", "100%")
                },
                RenderColumns = new List<ListRenderColumn>
                {
                    new ListRenderColumn(_localizer["name"], "userName"),
                    new ListRenderColumn(_localizer["roles"], "roles"),
                    new ListRenderColumn(_localizer["email"], "email"),
                    new ListRenderColumn(_localizer["created"], "created"),
                    new ListRenderColumn(_localizer["system_email_confirmed"], "emailConfirmed"),
                    new ListRenderColumn(_localizer["author"], "author")
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
                            new ActionParameter("callBackUrl", "/CompanyManage/Users")
                            {
                                IsCustomValue = true
                            }
                        },
                        ButtonType = BootstrapButton.Danger
                    }
                },
                Documentation = "This page allow to manage only your company users"
            };
            return model;
        }
    }
}