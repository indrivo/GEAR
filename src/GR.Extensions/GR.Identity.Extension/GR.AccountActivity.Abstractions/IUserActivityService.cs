using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GR.AccountActivity.Abstractions.Models;
using GR.AccountActivity.Abstractions.ViewModels;
using GR.Core;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using Microsoft.AspNetCore.Http;

namespace GR.AccountActivity.Abstractions
{
    public interface IUserActivityService
    {
        /// <summary>
        /// Is new device
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<bool> IsNewDeviceAsync(HttpContext context);


        /// <summary>
        /// Find device by ip
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        Task<ResultModel<UserDevice>> FindDeviceByIpAddressAsync(IPAddress address);

        /// <summary>
        /// Find current device 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<ResultModel<UserDevice>> FindDeviceAsync(HttpContext context);


        /// <summary>
        /// Get location of device
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        string GetLocationOfDevice(HttpContext context);

        /// <summary>
        /// Add new device
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> AddNewDeviceAsync(HttpContext context);

        /// <summary>
        /// Send mail to confirm new device
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userDevice"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<ResultModel> SendConfirmNewDeviceMailAsync(GearUser user, UserDevice userDevice, HttpContext context = null);

        /// <summary>
        /// Send mail to confirm new device
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<ResultModel> SendConfirmNewDeviceMailAsync(Guid? deviceId, HttpContext context = null);

        /// <summary>
        /// Confirm device
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<ResultModel> ConfirmDeviceAsync(Guid? deviceId, string code);

        /// <summary>
        /// Get confirmed devices
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<UserDevice>>> GetConfirmedDevicesAsync();

        /// <summary>
        /// Logout on all devices
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ResultModel> SignOutUserOnAllDevicesAsync(Guid? userId);

        /// <summary>
        /// Find device by id
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task<ResultModel<UserDevice>> FindDeviceByIdAsync(Guid? deviceId);

        /// <summary>
        /// Get user devices
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<DTResult<ConfirmedDevicesViewModel>> GetPagedConfirmedDevicesAsync(DTParameters parameters);

        /// <summary>
        /// Get paged user activity
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<DTResult<UserActivityViewModel>> GetPagedUserActivityAsync(DTParameters parameters);

        /// <summary>
        /// Delete all other confirmed and non confirmed
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteOtherConfirmedDevicesAsync(HttpContext context);

        /// <summary>
        /// Delete user device by id
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteUserDeviceAsync(Guid deviceId);

        /// <summary>
        /// Register user activity
        /// </summary>
        /// <param name="activityName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<ResultModel> RegisterUserActivityAsync(string activityName, HttpContext context);

        /// <summary>
        /// Get web sessions
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<DTResult<WebSessionViewModel>> GetWebSessionsAsync(DTParameters parameters);
    }
}