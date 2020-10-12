using AutoMapper;
using GR.AccountActivity.Abstractions.Models;
using GR.AccountActivity.Abstractions.ViewModels;
using GR.Core.Extensions;

namespace GR.AccountActivity.Abstractions.Helpers.MapperResolvers
{
    public class ConfirmedDeviceTimeMapperResolver<T> : IValueResolver<UserDevice, T, string> 
        where T : UserDeviceViewModel
    {
        public string Resolve(UserDevice source, T destination, string destMember,
            ResolutionContext context) => source.ConfirmDate.DisplayTextDate();
    }
}
