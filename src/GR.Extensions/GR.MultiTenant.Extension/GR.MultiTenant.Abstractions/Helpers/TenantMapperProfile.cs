using AutoMapper;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.MultiTenant.Abstractions.Helpers.MapperResolvers;
using GR.MultiTenant.Abstractions.ViewModels;

namespace GR.MultiTenant.Abstractions.Helpers
{
    public class TenantMapperProfile : Profile
    {
        public TenantMapperProfile()
        {
            CreateMap<Tenant, OrganizationListViewModel>()
                .ForMember(m => m.Users, o => o.MapFrom<OrganizationUsersCountMapperResolver>())
                .IncludeAllDerived()
                .ReverseMap();
        }
    }
}
