using AutoMapper;
using GR.Identity.Abstractions;
using GR.Identity.LdapAuth.Abstractions.Models;

namespace GR.Identity.LdapAuth.Abstractions.Helpers
{
    public class AdMapperProfile : Profile
    {
        public AdMapperProfile()
        {
            CreateMap<GearUser, LdapUser>()
                .IncludeAllDerived()
                .ReverseMap();
        }
    }
}