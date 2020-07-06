using System.Linq;
using AutoMapper;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.ViewModels.UserViewModels;

namespace GR.Identity.Helpers.MapperResolvers
{
    public class UserOrganizationMapperResolver : IValueResolver<GearUser, UserListItemViewModel, string>
    {
        #region Injectable

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly IIdentityContext _context;

        #endregion

        public UserOrganizationMapperResolver(IIdentityContext context)
        {
            _context = context;
        }

        public string Resolve(GearUser source, UserListItemViewModel destination, string destMember, ResolutionContext context)
        {
            var org = _context.Tenants.FirstOrDefault(x => x.Id == source.TenantId);
            return org?.Name;
        }
    }
}
