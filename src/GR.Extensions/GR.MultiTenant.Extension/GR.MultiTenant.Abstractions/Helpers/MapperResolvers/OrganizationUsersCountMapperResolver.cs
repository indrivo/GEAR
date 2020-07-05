using System.Linq;
using AutoMapper;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.MultiTenant.Abstractions.ViewModels;

namespace GR.MultiTenant.Abstractions.Helpers.MapperResolvers
{
    public class OrganizationUsersCountMapperResolver : IValueResolver<Tenant, OrganizationListViewModel, int>
    {
        #region Injectable

        /// <summary>
        /// Inject org. service
        /// </summary>
        private readonly IOrganizationService<Tenant> _organizationService;

        #endregion

        public OrganizationUsersCountMapperResolver(IOrganizationService<Tenant> organizationService)
        {
            _organizationService = organizationService;
        }

        public int Resolve(Tenant source, OrganizationListViewModel destination, int destMember, ResolutionContext context)
        {
            return _organizationService.GetUsersByOrganization(source).Count();
        }
    }
}
