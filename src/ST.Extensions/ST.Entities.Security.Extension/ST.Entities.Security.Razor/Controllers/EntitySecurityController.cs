using Microsoft.AspNetCore.Mvc;
using ST.Entities.Security.Abstractions;

namespace ST.Entities.Security.Razor.Controllers
{
    public class EntitySecurityController : Controller
    {
        /// <summary>
        /// Inject role access manager
        /// </summary>
        private readonly IEntityRoleAccessManager _accessManager;

        public EntitySecurityController(IEntityRoleAccessManager accessManager)
        {
            _accessManager = accessManager;
        }
    }
}
