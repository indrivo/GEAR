using GR.Identity.Profile.Abstractions;

namespace GR.Identity.Profile
{
    public class ProfileService : IProfileService
    {
        #region Injectable

        /// <summary>
        /// Inject profile context
        /// </summary>
        private readonly IProfileContext _profileContext;

        #endregion

        public ProfileService(IProfileContext profileContext)
        {
            _profileContext = profileContext;
        }
    }
}