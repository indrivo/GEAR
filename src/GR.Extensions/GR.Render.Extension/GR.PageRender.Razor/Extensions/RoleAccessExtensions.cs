using GR.Core;
using System.Collections.Generic;
using System.Linq;

namespace GR.PageRender.Razor.Extensions
{
    public static class RoleAccessExtensions
    {
        public static bool SortByUserRoleAccess(this object obj, IEnumerable<string> roles, string role)
        {
            if (roles.Contains(role)) return true;

            return !((BaseModel)obj).IsDeleted;
        }
    }
}