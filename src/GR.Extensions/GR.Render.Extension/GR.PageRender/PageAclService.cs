using System.Collections.Generic;
using System.Linq;
using GR.Core.Extensions;
using GR.Identity.Abstractions;
using GR.PageRender.Abstractions;
using GR.PageRender.Abstractions.Models.Pages;

namespace GR.PageRender
{
    public class PageAclService : IPageAclService
    {
        /// <summary>
        /// Check access
        /// </summary>
        /// <param name="page"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public bool HasAccess(Page page, IEnumerable<GearRole> roles)
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
