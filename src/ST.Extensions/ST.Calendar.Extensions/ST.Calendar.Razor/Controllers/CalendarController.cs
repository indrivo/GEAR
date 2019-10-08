using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ST.Calendar.Abstractions;
using ST.Calendar.Abstractions.Models;
using ST.Calendar.Abstractions.Models.ViewModels;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Identity.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ST.Calendar.Abstractions.Enums;
using ST.Calendar.Abstractions.ExternalProviders;
using ST.Calendar.Abstractions.Helpers.Mappers;
using ST.Calendar.Abstractions.Helpers.ServiceBuilders;
using ST.Identity.Abstractions.Models.MultiTenants;
using ST.Identity.Models.UserViewModels;
using ST.MultiTenant.Abstractions;

namespace ST.Calendar.Razor.Controllers
{
    [Authorize]
    public sealed class CalendarController : Controller
    {
        #region Injectable
        /// <summary>
        /// Inject Task service
        /// </summary>
        private readonly ICalendarManager _calendarManager;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Inject organization service
        /// </summary>
        private readonly IOrganizationService<Tenant> _organizationService;

        /// <summary>
        /// Settings
        /// </summary>
        private readonly JsonSerializerSettings _serializeSettings;

        #endregion

        public CalendarController(ICalendarManager calendarManager, IUserManager<ApplicationUser> userManager, IOrganizationService<Tenant> organizationService)
        {
            _calendarManager = calendarManager;
            _userManager = userManager;
            _organizationService = organizationService;
            _serializeSettings = CalendarServiceCollection.JsonSerializerSettings;
        }

        /// <summary>
        /// Internal calendar
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var factory = new ExternalCalendarProviderFactory();
            var providers = factory.GetProviders();
            var googleProvider = factory.CreateService(providers.ElementAt(0));
            var authRequest = await googleProvider.AuthorizeAsync(Guid.Parse("017326a6-29e0-4d49-84e6-2d52851bc33e"));
            if (authRequest.IsSuccess)
            {
                await googleProvider.PushEventAsync(new GetEventViewModel());
            }



            return View();
        }

        /// <summary>
        /// Add new event
        /// </summary>
        /// <param name="newEvent"></param>
        /// <returns></returns>
        [HttpPost, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<Guid>))]
        public async Task<JsonResult> AddEvent([Required]BaseEventViewModel newEvent)
        {
            var response = new ResultModel<Guid>();
            if (ModelState.IsValid) return Json(await _calendarManager.AddEventAsync(newEvent));
            response.Errors = ModelState.ToResultModelErrors().ToList();
            return Json(response, _serializeSettings);
        }

        /// <summary>
        /// Update event
        /// </summary>
        /// <param name="model"></param>
        /// <param name="organizer"></param>
        /// <returns></returns>
        [HttpPost, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> UpdateEvent([Required]UpdateEventViewModel model, Guid? organizer)
        {
            var response = new ResultModel();
            if (ModelState.IsValid) return Json(await _calendarManager.UpdateEventAsync(model, organizer));
            response.Errors = ModelState.ToResultModelErrors().ToList();
            return Json(response, _serializeSettings);
        }


        /// <summary>
        /// Change member acceptance
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="memberId"></param>
        /// <param name="acceptance"></param>
        /// <returns></returns>
        [HttpPost, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> ChangeMemberEventAcceptance(Guid? eventId, Guid? memberId, EventAcceptance acceptance = EventAcceptance.Tentative)
        {
            return Json(await _calendarManager.ChangeMemberEventAcceptanceAsync(eventId, memberId, acceptance));
        }


        /// <summary>
        /// Get my events
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<GetEventViewModel>>))]
        public async Task<JsonResult> GetAllEventsOrganizedByMe()
        {
            var eventRequest = await _calendarManager.GetAllEventsOrganizedByMeAsync();
            return Json(EventMapper.MapWithHelpers(eventRequest), _serializeSettings);
        }

        /// <summary>
        /// Get user event on interval for current organization
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<GetEventViewModel>>))]
        public async Task<JsonResult> GetOrganizationUserEvents(Guid? userId, [Required]DateTime startDate, [Required]DateTime endDate)
        {
            var response = new ResultModel<IEnumerable<CalendarEvent>>();
            if (!await _organizationService.IsUserPartOfOrganizationAsync(userId, _userManager.CurrentUserTenantId))
            {
                response.Errors.Add(new ErrorModel(string.Empty, "User not found or not assigned to this organization"));
                return Json(response);
            }

            var eventRequest = await _calendarManager.GetEventsAsync(userId, startDate, endDate);
            return Json(EventMapper.Map(eventRequest), _serializeSettings);
        }

        /// <summary>
        /// Get my events on by start and end date
        /// </summary>
        /// <param name="startDate"></param>, 
        /// <param name="endDate"></param>, 
        /// <returns></returns>
        [HttpGet, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<GetEventViewModel>>))]
        public async Task<JsonResult> GetMyEvents([Required]DateTime startDate, [Required]DateTime endDate)
        {
            var eventRequest = await _calendarManager.GetMyEventsAsync(startDate, endDate);
            return Json(EventMapper.MapWithHelpers(eventRequest), _serializeSettings);
        }

        /// <summary>
        /// Get events by time line
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="origin"></param>
        /// <param name="timeLineType"></param>
        /// <param name="expandDayPrecision"></param>
        /// <returns></returns>
        [HttpGet, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<GetEventViewModel>>))]
        public async Task<JsonResult> GetUserEventsByTimeLine(Guid? userId, DateTime? origin,
            CalendarTimeLineType timeLineType = CalendarTimeLineType.Month, int expandDayPrecision = 0)
        {
            var response = new ResultModel<IEnumerable<GetEventViewModel>>();
            if (!await _organizationService.IsUserPartOfOrganizationAsync(userId, _userManager.CurrentUserTenantId))
            {
                response.Errors.Add(new ErrorModel(string.Empty, "User not found or not assigned to this organization"));
                return Json(response);
            }

            var eventRequest = await _calendarManager.GetUserEventsByTimeLineAsync(userId, origin, timeLineType, expandDayPrecision);
            return Json(EventMapper.MapWithHelpers(eventRequest), _serializeSettings);
        }

        /// <summary>
        /// Get event by Id
        /// </summary>
        /// <param name="eventId">Event id</param>
        /// <returns></returns>
        [HttpGet, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<GetEventViewModel>))]
        public async Task<JsonResult> GetEventById([Required] Guid? eventId)
        {
            var eventRequest = await _calendarManager.GetEventByIdAsync(eventId);
            return Json(EventMapper.Map(eventRequest), _serializeSettings);
        }

        /// <summary>
        /// Delete event permanently
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>

        [HttpDelete, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> DeletePermanently(Guid? eventId) => Json(await _calendarManager.DeleteEventPermanentlyAsync(eventId));

        /// <summary>
        /// Delete event
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>

        [HttpDelete, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> DeleteLogically(Guid? eventId) => Json(await _calendarManager.DeleteEventLogicallyAsync(eventId));


        /// <summary>
        /// Restore event
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>

        [HttpDelete, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> Restore(Guid? eventId) => Json(await _calendarManager.RestoreEventLogicallyAsync(eventId));


        /// <summary>
        /// Helpers
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(Dictionary<string, Dictionary<int, string>>))]
        public JsonResult Helpers() => Json(new GetEventWithHelpersViewModel().Helpers);


        /// <summary>
        /// Get users
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(IEnumerable<ApplicationUser>))]
        public JsonResult GetOrganizationUsers()
        {
            var users = _organizationService.GetAllowedUsersByOrganizationId(_userManager.CurrentUserTenantId.GetValueOrDefault());
            return Json(users.Select(x => new SampleGetUserViewModel(x)));
        }
    }
}