using AutoMapper;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.ViewModels.UserViewModels;
using GR.Identity.Helpers.MapperResolvers;

namespace GR.Identity.Helpers
{
    public class IdentityMapperProfile : Profile
    {
        public IdentityMapperProfile()
        {
            //User info
            CreateMap<GearUser, UserInfoViewModel>()
                .IncludeAllDerived()
                .ReverseMap();

            //user list item
            CreateMap<GearUser, UserListItemViewModel>()
                .ForMember(m => m.CreatedBy, o => o.MapFrom(m => m.Author))
                .ForMember(m => m.CreatedDate, o => o.MapFrom(m => m.Created.ToShortDateString()))
                .ForMember(m => m.Changed, o => o.MapFrom(m => m.Changed.ToShortDateString()))
                .ForMember(m => m.Organization, o => o.MapFrom<UserOrganizationMapperResolver>())
                .ForMember(m => m.Roles, o => o.MapFrom<UserRolesMapperResolver>())
                .ForMember(m => m.Sessions, o => o.MapFrom<UserSessionsMapperResolver>())
                .IncludeAllDerived()
                .ReverseMap();
        }
    }
}