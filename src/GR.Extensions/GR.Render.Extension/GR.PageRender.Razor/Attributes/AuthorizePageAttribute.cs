using GR.Core.Extensions;
using GR.Identity.Abstractions;
using GR.PageRender.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GR.PageRender.Razor.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Attribute checks if User has required permission
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizePageAttribute : TypeFilterAttribute
    {
        public AuthorizePageAttribute() : base(typeof(AuthorizePageAttributeExecutor))
        {
            Arguments = new object[] { };
        }

        /// <summary>
        /// Executes permission check
        /// </summary>
        private class AuthorizePageAttributeExecutor : Attribute, IAsyncResourceFilter
        {
            private readonly IPageRender _pageRender;

            private readonly IPageAclService _pageAclService;

            private readonly RoleManager<GearRole> _roleManager;

            public AuthorizePageAttributeExecutor(IPageRender pageRender, IPageAclService pageAclService, RoleManager<GearRole> roleManager)
            {
                _pageRender = pageRender;
                _pageAclService = pageAclService;
                _roleManager = roleManager;
            }

            /// <inheritdoc />
            /// <summary>
            /// On Resource Executing async method
            /// Throws UnauthorizedResult if permission check is false
            /// </summary>
            /// <param name="context">Resource executing context</param>
            /// <param name="del">Resource executing delegate</param>
            /// <returns>Task</returns>
            public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate del)
            {
                var deniedResult = new RedirectToActionResult("AccessDenied", "Account", null);
                try
                {
                    var pageId = context.HttpContext.Request.Query.FirstOrDefault(x => x.Key == "pageId").Value
                        .ToString().ToGuid();
                    var roleNames = context.HttpContext.User.Claims
                        .Where(x => x.Type.Equals("role") || x.Type.EndsWith("role")).Select(x => x.Value).ToList();
                    var roles = roleNames.Select(async x => await _roleManager.FindByNameAsync(x)).Select(x => x.Result)
                        .ToList();

                    var page = await _pageRender.GetPageAsync(pageId);
                    if (page == null)
                    {
                        context.Result = deniedResult;
                        await context.Result.ExecuteResultAsync(context);
                    }

                    var hasAccess = _pageAclService.HasAccess(page, roles);
                    if (!hasAccess)
                    {
                        context.Result = deniedResult;
                        await context.Result.ExecuteResultAsync(context);
                    }
                    else await del();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    context.Result = deniedResult;
                    await context.Result.ExecuteResultAsync(context);
                }
            }
        }
    }
}