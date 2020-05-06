using AutoMapper;
using GR.AccountActivity.Abstractions.Models;
using GR.AccountActivity.Abstractions.ViewModels;
using GR.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace GR.AccountActivity.Abstractions.Helpers.MapperResolvers
{
    public class IsCurrentDeviceMapperResolver : IValueResolver<UserDevice, ConfirmedDevicesViewModel, bool>
    {
        #region Injectable

        /// <summary>
        /// Inject user activity
        /// </summary>
        private readonly IUserActivityService _userActivityService;

        /// <summary>
        /// Inject accessor
        /// </summary>
        private readonly IHttpContextAccessor _accessor;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userActivityService"></param>
        /// <param name="accessor"></param>
        public IsCurrentDeviceMapperResolver(IUserActivityService userActivityService, IHttpContextAccessor accessor)
        {
            _userActivityService = userActivityService;
            _accessor = accessor;
        }


        public bool Resolve(UserDevice source, ConfirmedDevicesViewModel destination, bool member, ResolutionContext context)
        {
            var currentDevice = _userActivityService.FindDeviceAsync(_accessor.HttpContext).ExecuteAsync();
            return source.Id.Equals(currentDevice.Result.Id);
        }
    }
}