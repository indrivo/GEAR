using AutoMapper;
using GR.AccountActivity.Abstractions.Helpers.MapperResolvers;
using GR.AccountActivity.Abstractions.Models;
using GR.AccountActivity.Abstractions.ViewModels;
using GR.Core;
using GR.Core.Extensions;

namespace GR.AccountActivity.Abstractions.Helpers
{
    public class AccountActivityMapperProfile : Profile
    {
        public AccountActivityMapperProfile()
        {
            CreateMap<UserDevice, ConfirmedDevicesViewModel>()
                .IncludeAllDerived()
                .ForMember(x => x.IsCurrent, o => o.MapFrom<IsCurrentDeviceMapperResolver>())
                .ForMember(x => x.ConfirmDateText, o => o.MapFrom<ConfirmedDeviceTimeMapperResolver>())
                .ReverseMap();


            CreateMap<UserActivity, UserActivityViewModel>()
                .IncludeAllDerived()
                .ForMember(x => x.UserId, o => o
                    .MapFrom(x => x.Device.UserId))
                .ForMember(x => x.Browser, o => o
                    .MapFrom(x => x.Device.Browser))
                .ForMember(x => x.IpAddress, o => o
                    .MapFrom(x => x.Device.IpAddress))
                .ForMember(x => x.Location, o => o
                    .MapFrom(x => x.Device.Location))
                .ForMember(x => x.Platform, o => o
                    .MapFrom(x => x.Device.Platform))
                .ForMember(x => x.When, o => o
                    .MapFrom(x => x.Created.DisplayTextDate(GearSettings.Date.DateFormatWithTime)))
                .ForMember(x => x.DeviceId, o => o
                    .MapFrom(x => x.DeviceId))
                .ForMember(x => x.Activity, o => o
                    .MapFrom(x => x.Activity))
                .ForMember(x => x.Source, o => o
                    .MapFrom(x => x.Device.Platform.StartsWith("Web") ? "web" : "mobile"))
                .ReverseMap();


            CreateMap<UserActivity, WebSessionViewModel>()
                .IncludeAllDerived()
                .ForMember(x => x.UserId, o => o
                    .MapFrom(x => x.Device.UserId))
                .ForMember(x => x.Browser, o => o
                    .MapFrom(x => x.Device.Browser))
                .ForMember(x => x.IpAddress, o => o
                    .MapFrom(x => x.Device.IpAddress))
                .ForMember(x => x.Location, o => o
                    .MapFrom(x => x.Device.Location))
                .ForMember(x => x.Platform, o => o
                    .MapFrom(x => x.Device.Platform))
                .ForMember(x => x.When, o => o
                    .MapFrom(x => x.Created.DisplayTextDate(GearSettings.Date.DateFormatWithTime)))
                .ForMember(x => x.DeviceId, o => o
                    .MapFrom(x => x.DeviceId))
                .ForMember(x => x.Activity, o => o
                    .MapFrom(x => x.Activity))
                .ForMember(x => x.Source, o => o
                    .MapFrom(x => x.Device.Platform.StartsWith("Web") ? "web" : "mobile"))
                .ReverseMap();
        }
    }
}