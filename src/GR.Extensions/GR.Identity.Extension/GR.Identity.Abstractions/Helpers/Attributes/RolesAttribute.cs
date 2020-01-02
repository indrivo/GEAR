using Microsoft.AspNetCore.Authorization;

namespace GR.Identity.Abstractions.Helpers.Attributes
{
    public class RolesAttribute : AuthorizeAttribute
    {
        public RolesAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }
}