using System.Collections.Generic;
using AutoMapper;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.ViewModels.UserViewModels;

namespace GR.Identity.Helpers.MapperResolvers
{
    public class UserRolesMapperResolver : IValueResolver<GearUser, UserListItemViewModel, IEnumerable<string>>
    {
        #region Injectable

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        #endregion

        public UserRolesMapperResolver(IUserManager<GearUser> userManager)
        {
            _userManager = userManager;
        }

        public IEnumerable<string> Resolve(GearUser source, UserListItemViewModel destination, IEnumerable<string> destMember, ResolutionContext context)
        {
            var roles = _userManager.UserManager.GetRolesAsync(source).GetAwaiter().GetResult();
            return roles;
        }
    }
}
