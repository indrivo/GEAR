using AutoMapper;
using GR.Subscriptions.Abstractions.Models;
using GR.Subscriptions.Abstractions.ViewModels;

namespace GR.Subscriptions.Abstractions.Helpers
{
    public class SubscriptionMapperProfile : Profile
    {
        public SubscriptionMapperProfile()
        {
            //Permission subscription 
            CreateMap<SubscriptionPermission, SubscriptionPermissionViewModel>()
                .ReverseMap();

            //Subscriptions
            CreateMap<Subscription, SubscriptionGetViewModel>()
                .ForMember(x => x.Permissions, 
                    o => o.MapFrom(x => x.SubscriptionPermissions))
                .ReverseMap();
        }
    }
}