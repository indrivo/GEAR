using AutoMapper;
using GR.Notifications.Abstractions.Models.Notifications;

namespace GR.Notifications.Abstractions.Helpers
{
    public class NotificationMapperProfile : Profile
    {
        public NotificationMapperProfile()
        {
            CreateMap<SystemNotifications, Notification>()
                .IncludeAllDerived()
                .ReverseMap();
        }
    }
}
