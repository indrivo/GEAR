using AutoMapper;
using GR.Calendar.Abstractions.Models;
using GR.Calendar.Abstractions.Models.ViewModels;
using GR.Calendar.Helpers.MapperResolvers;
using GR.Identity.Abstractions;

namespace GR.Calendar.Helpers
{
    public class CalendarMapperProfile : Profile
    {
        public CalendarMapperProfile()
        {
            CreateMap<GearUser, CalendarUserViewModel>()
                .IncludeAllDerived()
                .ReverseMap();

            CreateMap<CalendarEvent, GetEventViewModel>()
                .ForMember(m => m.OrganizerInfo, o => o.MapFrom<EventOrganizerInfoMapperResolver>())
                .ForMember(m => m.InvitedUsers, o => o.MapFrom<InvitedUsersMapperResolver>())
                .IncludeAllDerived()
                .ReverseMap();

            CreateMap<CalendarEvent, UpdateEventViewModel>()
                .IncludeAllDerived()
                .ReverseMap();
        }
    }
}
