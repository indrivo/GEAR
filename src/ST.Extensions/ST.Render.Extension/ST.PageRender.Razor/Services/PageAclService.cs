using System.Collections.Generic;
using System.Linq;
using ST.Core.Extensions;
using ST.Identity.Abstractions;
using ST.PageRender.Abstractions;
using ST.PageRender.Abstractions.Models.Pages;

namespace ST.PageRender.Razor.Services
{
    internal class PageAclService : IPageAclService
    {
        public bool HasAccess(Page page, IEnumerable<ApplicationRole> roles)
        {
            if (!page.IsEnabledAcl) return true;
            var acl = page.RolePagesAcls.ToList();
            var hasAccess = false;
            foreach (var role in roles)
            {
                var query = acl.FirstOrDefault(x => x.RoleId == role.Id.ToGuid());
                if (query == null) continue;
                if (query.AllowAccess)
                {
                    hasAccess = true;
                }
            }

            return hasAccess;
        }
    }
}
