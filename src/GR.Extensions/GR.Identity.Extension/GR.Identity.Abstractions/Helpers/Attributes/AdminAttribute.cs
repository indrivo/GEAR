using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers.Global;

namespace GR.Identity.Abstractions.Helpers.Attributes
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    public class AdminAttribute : GearAuthorizeAttribute
    {
        public AdminAttribute() : base(GearAuthenticationScheme.IdentityWithBearer)
        {
            Roles = GlobalResources.Roles.ADMINISTRATOR;
        }
    }
}