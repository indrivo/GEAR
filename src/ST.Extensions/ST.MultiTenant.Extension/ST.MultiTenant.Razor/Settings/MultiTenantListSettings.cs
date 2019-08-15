using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using ST.Core.Helpers;
using ST.Core.Razor.TagHelpers.TagHelperViewModels.ListTagHelperViewModels;
using ST.Core.Razor.TagHelpersStructures;

namespace ST.MultiTenant.Razor.Settings
{
    public class MultiTenantListSettings
    {
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
                    new ListRenderColumn(_localizer["created"], "created"),
                    new ListRenderColumn(_localizer["author"], "author")
                },
                HeadButtons = new List<UrlTagHelperViewModel>
                {
                    new UrlTagHelperViewModel
                    {
                        AspAction = "Create",
                        AspController = "CompanyManage",
                        ButtonName = "Add new user to company",
                        Description = "New user will be added to company"
                    },
                    new UrlTagHelperViewModel
                    {
                        AspAction = null,
                        AspController = "CompanyManage",
                        ButtonName = "Invite",
                        Description = null,
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
                        Name = _localizer["edit"],
                        Url = "/CompanyManage/LoadPageItems",
                        ButtonType = BootstrapButton.Primary
                    },
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