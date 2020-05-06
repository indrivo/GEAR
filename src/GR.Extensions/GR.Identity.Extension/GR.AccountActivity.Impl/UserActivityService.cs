using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using GR.AccountActivity.Abstractions;
using GR.AccountActivity.Abstractions.Events;
using GR.AccountActivity.Abstractions.Events.EventArgs;
using GR.AccountActivity.Abstractions.Helpers;
using GR.AccountActivity.Abstractions.Models;
using GR.AccountActivity.Abstractions.ViewModels;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.Core.Helpers.Templates;
using GR.Email.Abstractions;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Extensions;
using MaxMind.GeoIP2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UAParser;
using Wangkanai.Detection;

namespace GR.AccountActivity.Impl
{
    [Author(Authors.LUPEI_NICOLAE)]
    public class UserActivityService : IUserActivityService
    {
        #region Injectable

        /// <summary>
        /// Inject activity context
        /// </summary>
        private readonly IActivityContext _context;

        /// <summary>
        /// Inject detection service
        /// </summary>
        private readonly IDetection _detection;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject url helper
        /// </summary>
        private readonly IUrlHelper _urlHelper;

        /// <summary>
        /// Inject mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Inject http context
        /// </summary>
        private readonly IHttpContextAccessor _accessor;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="detection"></param>
        /// <param name="userManager"></param>
        /// <param name="urlHelper"></param>
        /// <param name="mapper"></param>
        /// <param name="accessor"></param>
        public UserActivityService(IActivityContext context, IDetection detection, IUserManager<GearUser> userManager, IUrlHelper urlHelper, IMapper mapper, IHttpContextAccessor accessor)
        {
            _context = context;
            _detection = detection;
            _userManager = userManager;
            _urlHelper = urlHelper;
            _mapper = mapper;
            _accessor = accessor;
        }

        /// <summary>
        /// Add new device
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> AddNewDeviceAsync(HttpContext context)
        {
            Arg.NotNull(context, nameof(AddNewDeviceAsync));
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess) return userRequest.Map<Guid>();
            var user = userRequest.Result;

            var ip = context.Connection.RemoteIpAddress;
            var (platform, browser) = ExtractPlatformAndBrowserVersions(context);

            var device = new UserDevice
            {
                IpAddress = ip.ToString(),
                Location = GetLocationOfDevice(context),
                UserId = user.Id,
                Browser = browser,
                Platform = platform
            };


            await _context.Devices.AddAsync(device);
            var dbResult = await _context.PushAsync();
            if (dbResult.IsSuccess) await SendConfirmNewDeviceMailAsync(user, device, context);

            return dbResult.Map(device.Id);
        }

        /// <summary>
        /// Extract platform and browser
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private (string, string) ExtractPlatformAndBrowserVersions(HttpContext context)
        {
            var response = (string.Empty, string.Empty);
            var uaParser = Parser.GetDefault();
            var clientInfo = uaParser.Parse(context.Request.Headers["User-Agent"]);

            if (context.Request.Headers.ContainsKey("REQ-MOBILE"))
            {
                var model = context.Request.Headers["REQ-MOBILE"];
                var os = $"{clientInfo.OS.Family} {clientInfo.OS.Major}";
                var platform = $"Mobile App {_detection.Device.Type} ({os})";

                response.Item1 = platform;
                response.Item2 = model;
            }
            else
            {
                var os = $"{clientInfo.OS.Family} {clientInfo.OS.Major}";
                var platform = $"Web {_detection.Device.Type} ({os})";
                var browser = $"{clientInfo.UA.Family} {clientInfo.UA.Major}";

                response.Item1 = platform;
                response.Item2 = browser;
            }

            return response;
        }

        /// <summary>
        /// is new device
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task<bool> IsNewDeviceAsync(HttpContext context)
        {
            Arg.NotNull(context, nameof(IsNewDeviceAsync));
            var ip = context.Connection.RemoteIpAddress;
            var checkDevice = await FindDeviceByIpAddressAsync(ip);
            if (checkDevice.IsSuccess) return !checkDevice.Result.IsConfirmed;

            return false;
        }

        /// <summary>
        /// Find device by 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<UserDevice>> FindDeviceByIpAddressAsync(IPAddress address)
        {
            var device = await _context.Devices
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IpAddress.Equals(address.ToString()));
            if (device == null) return new NotFoundResultModel<UserDevice>();

            return new SuccessResultModel<UserDevice>(device);
        }

        /// <summary>
        /// Find device
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<UserDevice>> FindDeviceAsync(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress;
            var location = GetLocationOfDevice(context);
            var (platform, browser) = ExtractPlatformAndBrowserVersions(context);
            var userId = _userManager.FindUserIdInClaims();

            var device = await _context.Devices
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IpAddress.Equals(ip.ToString())
                                          && x.Platform.Equals(platform)
                                          && x.Location.Equals(location)
                                          && x.UserId.Equals(userId.Result)
                                          && x.Browser.Equals(browser));

            if (device == null) return new NotFoundResultModel<UserDevice>();

            return new SuccessResultModel<UserDevice>(device);
        }

        /// <summary>
        /// Get location of device
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual string GetLocationOfDevice(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress;
            if (ipAddress.IsLocalIpAddress()) return ipAddress.ToString();

            var path = Path.Combine(AppContext.BaseDirectory, "Configuration\\GeoLite2-City.mmdb");
            try
            {
                using (var reader = new DatabaseReader(path))
                {
                    var req = reader.City(ipAddress);
                    return $"{req.Country.Name}, {req.City.Name ?? req.Location.TimeZone}";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return ipAddress.ToString();
        }

        /// <summary>
        /// Send confirm new device mail
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userDevice"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SendConfirmNewDeviceMailAsync(GearUser user, UserDevice userDevice, HttpContext context = null)
        {
            var result = new ResultModel();
            if (userDevice.IsConfirmed)
            {
                result.AddError("The device has already been confirmed");
                return result;
            }

            const string subject = "Confirm new device";
            var templateReq = TemplateManager.GetTemplateBody("confirm_new_device");
            if (!templateReq.IsSuccess) return result;

            var code = await _userManager.UserManager.GenerateUserTokenAsync(user, AccountActivityResources.TrackActivityTokenProvider, AccountActivityResources.ConfirmDevicePurpose);
            var url = _urlHelper.ActionLink("ConfirmDevice", "AccountActivity",
            new
            {
                deviceId = userDevice.Id,
                code
            }, context);

            var location = $"({userDevice.IpAddress}) {userDevice.Location}";
            var body = templateReq.Result.Inject(new Dictionary<string, string>
            {
                { "Link", url },
                { "Location", location },
                { "Device", userDevice.Platform }
            });

            GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async x =>
            {
                var emailSender = x.InjectService<IEmailSender>();
                await emailSender.SendEmailAsync(user.Email, subject, body);
            });

            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// Send Confirm New Device to Mail 
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SendConfirmNewDeviceMailAsync(Guid? deviceId, HttpContext context = null)
        {
            var result = new ResultModel();
            if (deviceId == null) return result;
            var device = await _context.Devices.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(deviceId));
            if (device == null) return result;
            var user = await _userManager.UserManager.FindByIdAsync(device.UserId.ToString());
            if (user == null) return result;
            return await SendConfirmNewDeviceMailAsync(user, device, context);
        }

        /// <summary>
        /// Confirm device
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> ConfirmDeviceAsync(Guid? deviceId, string code)
        {
            if (deviceId == null || code.IsNullOrEmpty()) return new InvalidParametersResultModel();

            var device = await _context.Devices.FirstOrDefaultAsync(x => x.Id.Equals(deviceId));
            if (device == null) return new ResultModel()
                .AddError("Device not found")
                .ToBase();

            if (device.IsConfirmed)
            {
                var res = new ResultModel
                {
                    IsSuccess = true
                };
                res.AddError("Device is already confirmed");

                return res;
            }

            var user = await _userManager.UserManager.FindByIdAsync(device.UserId.ToString());
            var isValid = await _userManager.UserManager.VerifyUserTokenAsync(user, AccountActivityResources.TrackActivityTokenProvider,
                AccountActivityResources.ConfirmDevicePurpose, code);

            if (!isValid) return new ResultModel()
               .AddError("Invalid token")
               .ToBase();

            device.IsConfirmed = true;
            device.ConfirmDate = DateTime.Now;
            _context.Devices.Update(device);
            var dbResult = await _context.PushAsync();
            if (dbResult.IsSuccess)
            {
                //trigger confirm new device
                AccountActivityEvents.Events.DeviceConfirmed(new DeviceConfirmedEventArgs
                {
                    DeviceId = deviceId.Value,
                    Device = device,
                    HttpContext = _accessor.HttpContext
                });
            }
            return dbResult;
        }

        /// <summary>
        /// Get confirmed devices
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<UserDevice>>> GetConfirmedDevicesAsync()
        {
            var user = (await _userManager.GetCurrentUserAsync()).Result;
            if (user == null) return new NotAuthorizedResultModel<IEnumerable<UserDevice>>();

            var devices = await _context.Devices.AsNoTracking().Where(x => x.UserId.Equals(user.Id) && x.IsConfirmed)
                .ToListAsync();
            return new SuccessResultModel<IEnumerable<UserDevice>>(devices);
        }

        /// <summary>
        /// Logout user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SignOutUserOnAllDevicesAsync(Guid? userId)
        {
            if (userId == null) return new InvalidParametersResultModel();
            var user = await _userManager.UserManager.FindByIdAsync(userId.ToString());
            var result = await _userManager.UserManager.UpdateSecurityStampAsync(user);
            if (result.Succeeded) return new SuccessResultModel<object>().ToBase();
            var errResponse = new ResultModel();
            errResponse.AppendIdentityErrors(result.Errors);
            return errResponse;
        }

        /// <summary>
        /// Find device by id
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<UserDevice>> FindDeviceByIdAsync(Guid? deviceId)
        {
            if (deviceId == null) return new NotFoundResultModel<UserDevice>();
            return await _context.FindByIdAsync<UserDevice, Guid>(deviceId.Value);
        }

        /// <summary>
        /// Get confirmed devices
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<DTResult<ConfirmedDevicesViewModel>> GetPagedConfirmedDevicesAsync(DTParameters parameters)
        {
            var userId = _userManager.FindUserIdInClaims().Result;
            var paginationResponse = await _context.Devices
                .Where(x => x.IsConfirmed && x.UserId.Equals(userId))
                .OrderByDescending(x => x.ConfirmDate)
                .GetPagedAsDtResultAsync(parameters);

            var mapped = _mapper.Map<IEnumerable<ConfirmedDevicesViewModel>>(paginationResponse.Data);
            return paginationResponse.MapResult(mapped);
        }

        /// <summary>
        /// Get paged user activity
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<DTResult<UserActivityViewModel>> GetPagedUserActivityAsync(DTParameters parameters)
        {
            var userId = _userManager.FindUserIdInClaims().Result;
            var paginationResponse = await _context.Activities
                .Include(x => x.Device)
                .Where(x => x.Device.UserId.Equals(userId))
                .OrderByDescending(x => x.Created)
                .GetPagedAsDtResultAsync(parameters);

            var mapped = _mapper.Map<IEnumerable<UserActivityViewModel>>(paginationResponse.Data);
            return paginationResponse.MapResult(mapped);
        }

        /// <summary>
        /// Get web session
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<DTResult<WebSessionViewModel>> GetWebSessionsAsync(DTParameters parameters)
        {
            var userId = _userManager.FindUserIdInClaims().Result;
            var paginationResponse = await _context.Activities
                .Include(x => x.Device)
                .Where(x => x.Device.UserId.Equals(userId)
                            && x.Device.Platform.StartsWith("Web")
                            && x.Activity.Equals(AccountActivityResources.ActivityTypes.SIGNIN))
                .OrderByDescending(x => x.Created)
                .GetPagedAsDtResultAsync(parameters);

            var mapped = _mapper.Map<IEnumerable<WebSessionViewModel>>(paginationResponse.Data);
            return paginationResponse.MapResult(mapped);
        }

        /// <summary>
        /// Delete all other confirmed and non confirmed
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> DeleteOtherConfirmedDevicesAsync(HttpContext context)
        {
            var currentGet = await FindDeviceAsync(context);
            if (!currentGet.IsSuccess) return new NotAuthorizedResultModel<object>().ToBase();
            var device = currentGet.Result;

            var other = await _context.Devices
                .Where(x => x.UserId.Equals(device.UserId) && x.Id != device.Id)
                .ToListAsync();

            _context.Devices.RemoveRange(other);
            return await _context.PushAsync();
        }

        /// <summary>
        /// Delete device
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> DeleteUserDeviceAsync(Guid deviceId)
        {
            var deviceGet = await _context.FindByIdAsync<UserDevice, Guid>(deviceId);
            if (!deviceGet.IsSuccess) return deviceGet.ToBase();
            var userIdGet = _userManager.FindUserIdInClaims();
            if (!userIdGet.IsSuccess) return userIdGet.ToBase();

            if (deviceGet.Result.UserId != userIdGet.Result)
            {
                var response = new ResultModel();
                response.AddError("This is not your device and can't be removed");
                return response;
            }

            _context.Devices.Remove(deviceGet.Result);
            return await _context.PushAsync();
        }

        /// <summary>
        /// Register user activity
        /// </summary>
        /// <param name="activityName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResultModel> RegisterUserActivityAsync(string activityName, HttpContext context)
        {
            if (activityName.IsNullOrEmpty()) return new InvalidParametersResultModel();
            if (context == null) context = _accessor.HttpContext;
            var deviceGet = await FindDeviceAsync(context);
            if (!deviceGet.IsSuccess) return deviceGet.ToBase();

            var activity = new UserActivity
            {
                Activity = activityName,
                DeviceId = deviceGet.Result.Id
            };

            await _context.Activities.AddAsync(activity);

            return await _context.PushAsync();
        }
    }
}