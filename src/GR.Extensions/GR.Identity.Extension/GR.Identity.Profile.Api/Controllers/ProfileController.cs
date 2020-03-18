using GR.Core.Razor.BaseControllers;
using GR.Identity.Profile.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.Identity.Profile.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class ProfileController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject profile service
        /// </summary>
        private readonly IProfileService _profileService;

        #endregion

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }


    }
}
