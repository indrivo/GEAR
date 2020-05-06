using AutoMapper;
using GR.AccountActivity.Abstractions.Models;
using GR.AccountActivity.Abstractions.ViewModels;
using GR.Core.Extensions;

namespace GR.AccountActivity.Abstractions.Helpers.MapperResolvers
{
    public class ConfirmedDeviceTimeMapperResolver : IValueResolver<UserDevice, ConfirmedDevicesViewModel, string>
    {
        public string Resolve(UserDevice source, ConfirmedDevicesViewModel destination, string destMember,
            ResolutionContext context) => source.ConfirmDate.DisplayTextDate();
    }
}
