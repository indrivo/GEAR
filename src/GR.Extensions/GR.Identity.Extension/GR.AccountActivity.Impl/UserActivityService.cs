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
using GR.AccountActivity.Impl.Models;
using GR.Cache.Abstractions;
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
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UAParser;

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
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Inject http context
        /// </summary>
        private readonly IHttpContextAccessor _accessor;

        /// <summary>
        /// Inject logger
        /// </summary>
        private readonly ILogger<UserActivityService> _logger;

        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly ICacheService _cacheService;

        /// <summary>
        /// Inject link generator
        /// </summary>
        private readonly LinkGenerator _generator;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="mapper"></param>
        /// <param name="accessor"></param>
        /// <param name="logger"></param>
        /// <param name="cacheService"></param>
        /// <param name="generator"></param>
        public UserActivityService(IActivityContext context, IUserManager<GearUser> userManager, IMapper mapper, IHttpContextAccessor accessor, ILogger<UserActivityService> logger, ICacheService cacheService, LinkGenerator generator)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _accessor = accessor;
            _logger = logger;
            _cacheService = cacheService;
            _generator = generator;
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

            var ip = GetIpAddress(context);
            var platformRequest = ExtractPlatformAndBrowserVersions(context);
            if (!platformRequest.IsSuccess) return platformRequest.Map<Guid>();

            var device = new UserDevice
            {
                IpAddress = ip.ToString(),
                Location = GetLocationOfDevice(context),
                UserId = user.Id,
                Browser = platformRequest.Result.Browser,
                Platform = platformRequest.Result.Platform
            };


            await _context.Devices.AddAsync(device);
            var dbResult = await _context.PushAsync();
            if (dbResult.IsSuccess) await SendConfirmNewDeviceMailAsync(user, device, context);

            return dbResult.Map(device.Id);
        }

        /// <summary>
        /// is new device
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task<bool> IsNewDeviceAsync(HttpContext context)
        {
            Arg.NotNull(context, nameof(IsNewDeviceAsync));
            var ip = GetIpAddress(context);
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
            var userIdReq = _userManager.FindUserIdInClaims();
            if (!userIdReq.IsSuccess) return userIdReq.Map<UserDevice>();

            return await FindDeviceAsync(userIdReq.Result, context);
        }

        /// <summary>
        /// Find device
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<UserDevice>> FindDeviceAsync(Guid userId, HttpContext context)
        {
            if (context.Request.Headers.ContainsKey(AccountActivityResources.DeviceIdHeader)
                && context.Request.Headers[AccountActivityResources.DeviceIdHeader].ToString().IsGuid())
            {
                var deviceId = context.Request.Headers[AccountActivityResources.DeviceIdHeader].ToString().ToGuid();
                var deviceFromId = await _context.Devices
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id.Equals(deviceId) && x.UserId.Equals(userId));
                if (deviceFromId != null)
                {
                    return new SuccessResultModel<UserDevice>(deviceFromId);
                }
            }

            var ip = GetIpAddress(context);
            var location = GetLocationOfDevice(context);
            var platformRequest = ExtractPlatformAndBrowserVersions(context);
            if (!platformRequest.IsSuccess) return platformRequest.Map<UserDevice>();

            var key = AccountActivityResources.GetDeviceCacheKey(userId, ip.ToString(), platformRequest.Result.Platform, location, platformRequest.Result.Browser);
            var device = await _cacheService.GetOrSetResponseAsync(key, async () => await _context.Devices
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IpAddress.Equals(ip.ToString())
                                          && x.Platform.Equals(platformRequest.Result.Platform)
                                          && x.Location.Equals(location)
                                          && x.UserId.Equals(userId)
                                          && x.Browser.Equals(platformRequest.Result.Browser)));

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
            var ipAddress = GetIpAddress(context);
            if (ipAddress.IsLocalIpAddress()) return ipAddress.ToString();
            if (ipAddress.IsInternal()) return ipAddress.ToString();

            var path = Path.Combine(AppContext.BaseDirectory, "Configuration/GeoLite2-City.mmdb");
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
                _logger.LogError(e, $"Fail to detect ip address location, ip: {ipAddress}");
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
            var url = _generator.GetUriByAction(context, "ConfirmDevice", "AccountActivity", new
            {
                deviceId = userDevice.Id,
                code
            });

            var location = $"({userDevice.IpAddress}) {userDevice.Location}";
            var body = templateReq.Result.Inject(new Dictionary<string, string>
            {
                { "Link", url },
                { "Location", location },
                { "Device", userDevice.Platform }
            });

            GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async (serviceProvider, cancellationToken) =>
            {
                var emailSender = serviceProvider.GetService<IEmailSender>();
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

                await _cacheService.RemoveAsync(device.GetDeviceCacheKey());
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
            return await GetPagedUserActivityAsync(parameters, userId);
        }

        /// <summary>
        /// Get paged user activities fro user
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<DTResult<UserActivityViewModel>> GetPagedUserActivityAsync(DTParameters parameters, Guid userId)
        {
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
            var dbResponse = await _context.PushAsync();
            if (!dbResponse.IsSuccess) return dbResponse;
            foreach (var otherDevice in other) await _cacheService.RemoveAsync(otherDevice.GetDeviceCacheKey());
            return dbResponse;
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
            var dbResponse = await _context.PushAsync();
            if (dbResponse.IsSuccess)
            {
                await _cacheService.RemoveAsync(deviceGet.Result.GetDeviceCacheKey());
            }
            return dbResponse;
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

            var dbResponse = await _context.PushAsync();
            if (!dbResponse.IsSuccess)
            {
                _logger.LogError($"Fail to save user activity errors => {dbResponse.SerializeAsJson()}, entry => {activity.SerializeAsJson()}");
            }

            return dbResponse;
        }

        /// <summary>
        /// Register user activity 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="activityName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> RegisterUserNotAuthenticatedActivityAsync(Guid userId, string activityName, HttpContext context)
        {
            if (activityName.IsNullOrEmpty()) return new InvalidParametersResultModel();
            if (context == null) context = _accessor.HttpContext;
            var deviceGet = await FindDeviceAsync(userId, context);
            if (!deviceGet.IsSuccess)
            {
                _logger.LogError($"Method: {nameof(RegisterUserNotAuthenticatedActivityAsync)} Can't detect device for register user {userId} {activityName} activity, context: {context.Request.Headers.SerializeAsJson()}");
                return deviceGet.ToBase();
            }

            var activity = new UserActivity
            {
                Activity = activityName,
                DeviceId = deviceGet.Result.Id
            };

            await _context.Activities.AddAsync(activity);

            var dbResponse = await _context.PushAsync();
            if (!dbResponse.IsSuccess)
            {
                _logger.LogError($"Fail to save user activity errors => {dbResponse.SerializeAsJson()}, entry => {activity.SerializeAsJson()}");
            }
            return dbResponse;
        }

        #region Helpers

        /// <summary>
        /// Extract platform and browser
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private ResultModel<ExtractedInfoFromHttpContext> ExtractPlatformAndBrowserVersions(HttpContext context)
        {
            var response = new ResultModel<ExtractedInfoFromHttpContext>();
            var uaParser = Parser.GetDefault();
            if (!context.Request.Headers.ContainsKey("User-Agent"))
            {
                response.AddError("G-UserAgent-Missing", "User agent is not defined and can't extract device info");
                return response;
            }

            var clientInfo = uaParser.Parse(context.Request.Headers["User-Agent"]);
            if (context.Request.Headers.ContainsKey("REQ-MOBILE"))
            {
                var model = context.Request.Headers["REQ-MOBILE"];
                var os = $"{clientInfo.OS.Family} {clientInfo.OS.Major}";
                var platform = $"Mobile App ({os})";
                response.Result = new ExtractedInfoFromHttpContext
                {
                    Browser = model,
                    IsMobile = true,
                    Platform = platform
                };
                response.IsSuccess = true;
            }
            else
            {
                if (clientInfo.String.StartsWith("Postman"))
                {
                    response.Result = new ExtractedInfoFromHttpContext
                    {
                        Browser = "Postman",
                        Platform = "Postman"
                    };
                }
                else
                {
                    var os = $"{clientInfo.OS.Family} {clientInfo.OS.Major}";
                    var model = clientInfo.Device.Model != "Other" ? clientInfo.Device.Model : string.Empty;
                    var platform = $"Web {model} ({os})";
                    var browser = $"{clientInfo.UA.Family} {clientInfo.UA.Major}";
                    response.Result = new ExtractedInfoFromHttpContext
                    {
                        Browser = browser,
                        Platform = platform
                    };
                }

                response.IsSuccess = true;
            }

            return response;
        }


        /// <summary>
        /// Get ip address
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private IPAddress GetIpAddress(HttpContext context)
        {
            const string forwardAddress = "X-Forwarded-For";
            var ipAddress = context.Connection.RemoteIpAddress;

            if (ipAddress.IsLocalIpAddress())
            {
                _logger.LogWarning($"Remote ip address is local, ip: {ipAddress}");
                return ipAddress;
            }

            if (!ipAddress.ToString().StartsWith("192")) return ipAddress;

            if (context.Request.Headers.ContainsKey(forwardAddress))
            {
                ipAddress = IPAddress.Parse(context.Request.Headers[forwardAddress]);
            }

            return ipAddress;
        }

        #endregion
    }
}